using System.ComponentModel;
using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.Communication;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;
using System;
using System.Text.RegularExpressions;
using System.Timers;
using Elve.Driver.BenqProjector.Annotations;

namespace Elve.Driver.BenqProjector
{
    /// <summary>
    /// Projector Protocol Service
    /// </summary>
    internal class ProjectorProtocol : INotifyPropertyChanged
    {

        #region Private Fields

        private const string AspectStatusQuery = "*ASP=?#";
        private const string CurrentSourceQuery = "*SOUR=?#";
        private const string PictureModeQuery = "*APPMOD=?#";
        private const string PowerStateOff = "*POW=OFF#";
        private const string PowerStateOn = "*POW=ON#";
        private const string PowerStateQuery = "*POW=?#";
        private const string Unavailable = "*Block item#";
        private const string LampHoursQuery = "*LTIM=?#";

        private static readonly Regex AspectStatusResponse = new Regex(@"^\*ASP=(4:3|16:9|16:10|AUTO|REAL|LBOX|WIDE|ANAM)#");
        private static readonly Regex CurrentSourceResponse = new Regex(@"^\*SOUR=(RGB|RGB2|YPBR|DVIA|DVID|HDMI|HDMI2|VID|SVID|NETWORK|USBDISPLAY|USBREADER)#");
        private static readonly Regex PictureModeResponse = new Regex(@"\*APPMOD=(DYNAMIC|PRESET|SRGB|BRIGHT|LIVINGROOM|GAME|CINE|STD|USER1|USER2|USER3)#");
        private static readonly Regex PowerStateResponse = new Regex(@"^\*POW=(ON|OFF)#");
        private static readonly Regex LampHoursResponse = new Regex(@"^\*LTIM=(\d+)#");

        private readonly CommandResponseQueue<string, Regex> _commandQueue;
        private readonly ICommunication _communication;
        private readonly ILogger _logger;
        private readonly ProjectorModel _model;
        private readonly Timer _watchdogTimer;
        private bool _isIdle;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectorProtocol" /> class.
        /// </summary>
        /// <param name="communication">The communication.</param>
        /// <param name="model">The model to be updated.</param>
        public ProjectorProtocol(ICommunication communication, ProjectorModel model, ILogger logger)
        {
            _communication = communication;
            _model = model;
            _logger = logger;
            _commandQueue = new CommandResponseQueue<string, Regex>(10);
            _watchdogTimer = new Timer { AutoReset = false };
            _watchdogTimer.Elapsed += OnCommandWatchdogTimeout;
            _watchdogTimer.Interval = 3000;
            _communication.ReceivedDelimitedString += ProjectorReceivedEvent;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public bool IsIdle
        {
            get { return _isIdle; }
            set
            {
                if (value.Equals(_isIdle)) return;
                _isIdle = value;
                OnPropertyChanged("IsIdle");
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the aspect ratio.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        public void SetAspectRatio(string aspectRatio)
        {
            EnqueueCommand(AspectStatusQuery.Replace("?", aspectRatio), AspectStatusResponse);
        }

        /// <summary>
        /// Sets the current source.
        /// </summary>
        /// <param name="source">The source.</param>
        public void SetCurrentSource(string source)
        {
            EnqueueCommand(CurrentSourceQuery.Replace("?", source), CurrentSourceResponse);
        }

        /// <summary>
        /// Sets the picture mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public void SetPictureMode(string mode)
        {
            EnqueueCommand(PictureModeQuery.Replace("?", mode), PictureModeResponse);
        }

        /// <summary>
        /// Sets the state of the power.
        /// </summary>
        /// <param name="on">if set to <c>true</c> [on].</param>
        public void SetPowerState(bool on)
        {
            EnqueueCommand(@on ? PowerStateOn : PowerStateOff, PowerStateResponse);
        }

        /// <summary>
        /// Updates the status of all fields.
        /// </summary>
        public void UpdateStatus()
        {
            EnqueueCommand(ProjectorProtocol.PowerStateQuery, ProjectorProtocol.PowerStateResponse);

            if (_model.LampHours.ToPrimitiveInt32() == 0)
            {
                EnqueueCommand(ProjectorProtocol.LampHoursQuery, ProjectorProtocol.LampHoursResponse);
            }

            if (_model.PowerState)
            {
                EnqueueCommand(ProjectorProtocol.AspectStatusQuery, ProjectorProtocol.AspectStatusResponse);
                EnqueueCommand(ProjectorProtocol.CurrentSourceQuery, ProjectorProtocol.CurrentSourceResponse);
                EnqueueCommand(ProjectorProtocol.PictureModeQuery, ProjectorProtocol.PictureModeResponse);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods

        #region Private Methods

        private void EnqueueCommand(string cmd, Regex response)
        {
            _commandQueue.Enqueue(cmd, response);

            if (_commandQueue.ResponseCount == 1)
                SendCommand(_commandQueue.DequeueCommand());
        }

        private void OnCommandWatchdogTimeout(object sender, ElapsedEventArgs e)
        {
            _logger.Warning("Timeout waiting for command response from projector. Resetting queues.");
            _commandQueue.Clear();
        }

        protected virtual void ProcessProjectorResponse(string rawResponse)
        {
            _logger.DebugFormat("Processing response: {0}", rawResponse);

            if (rawResponse == Unavailable) return;

            var match = PowerStateResponse.Match(rawResponse);
            if (match.Success)
            {
                var power = match.Groups[1].Value == "ON";
                _model.PowerState = new ScriptBoolean(power);
                return;
            }

            match = AspectStatusResponse.Match(rawResponse);
            if (match.Success)
            {
                var state = match.Groups[1].Value;
                var index = Array.IndexOf(ProjectorModel.AspectRatioNames, state);
                _model.AspectRatio = new ScriptNumber(index);
                return;
            }

            match = CurrentSourceResponse.Match(rawResponse);
            if (match.Success)
            {
                var state = match.Groups[1].Value;
                var index = Array.IndexOf(ProjectorModel.SourceTypeNames, state);
                _model.CurrentSource = new ScriptNumber(index);
                return;
            }

            match = PictureModeResponse.Match(rawResponse);
            if (match.Success)
            {
                var state = match.Groups[1].Value;
                var index = Array.IndexOf(ProjectorModel.PictureModeNames, state);
                _model.PictureMode = new ScriptNumber(index);
            }

            match = LampHoursResponse.Match(rawResponse);
            if (match.Success)
            {
                var hours = double.Parse(match.Groups[1].Value);
                _model.LampHours = new ScriptNumber(hours);
            }
        }

        private void ProjectorReceivedEvent(object sender, ReceivedDelimitedStringEventArgs e)
        {
            if (e.RawResponse.Length == 0) return;
            _logger.DebugFormat("RX: {0}", e.RawResponse);

            try
            {
                _watchdogTimer.Stop();
                if (_commandQueue.IsResponseEmpty()) return;

                var expected = _commandQueue.PeekResponse();
                if (e.RawResponse == ProjectorProtocol.Unavailable || expected.IsMatch(e.RawResponse))
                {
                    _commandQueue.DequeueResponse();

                    ProcessProjectorResponse(e.RawResponse);

                    if (!_commandQueue.IsCommandEmpty())
                        SendCommand(_commandQueue.DequeueCommand());

                    IsIdle = _commandQueue.IsResponseEmpty();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("An error ocurred in the driver received response event.  Error: {0}", ex);
            }
        }

        private void SendCommand(string cmd)
        {
            _logger.DebugFormat("TX: {0}", cmd);
            _communication.Send("\r" + cmd + "\r");
            _watchdogTimer.Start();
        }

        #endregion Private Methods

    }
}