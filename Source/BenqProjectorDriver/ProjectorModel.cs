using System;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;
using Elve.Driver.BenqProjector.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Elve.Driver.BenqProjector
{
    internal class ProjectorModel : INotifyPropertyChanged
    {
        #region Public Fields

        /// <summary>
        /// The aspect ratio names
        /// </summary>
        public static readonly string[] AspectRatioNames =
        {
            "n/a",
            "4x3",
            "16x9",
            "16:10",
            "AUTO",
            "REAL",
            "LBOX",
            "WIDE",
            "ANAM"
        };

        /// <summary>
        /// The picture mode names
        /// </summary>
        public static readonly string[] PictureModeNames =
        {
            "n/a",
            "DYNAMIC",
            "PRESET",
            "SRGB",
            "BRIGHT",
            "LIVINGROOM",
            "GAME",
            "CINE",
            "STD",
            "USER1",
            "USER2",
            "USER3"
        };

        /// <summary>
        /// The source type names
        /// </summary>
        public static readonly string[] SourceTypeNames =
        {
            "n/a",
            "RGB",
            "RGB2",
            "YPBR",
            "DVIA",
            "DVID",
            "HDMI",
            "HDMI2",
            "VID",
            "SVID",
            "NETWORK",
            "USBDISPLAY",
            "USBREADER"
        };

        #endregion Public Fields

        #region Private Fields

        private ScriptNumber _aspectRatio = new ScriptNumber();

        private ScriptNumber _currentSource = new ScriptNumber();

        private ScriptNumber _pictureMode = new ScriptNumber();

        private ScriptBoolean _powerState = new ScriptBoolean();

        private ScriptNumber _lampHours = new ScriptNumber();

        #endregion Private Fields

        #region Public Events


        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>
        /// The aspect ratio.
        /// </value>
        public ScriptNumber AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                if (Equals(value, _aspectRatio)) return;
                _aspectRatio = value;
                OnPropertyChanged("AspectRatio");
            }
        }

        /// <summary>
        /// Gets or sets the lamp hours.
        /// </summary>
        /// <value>
        /// The lamp hours.
        /// </value>
        public ScriptNumber LampHours
        {
            get { return _lampHours; }
            set
            {
                if (Equals(value, _lampHours)) return;
                _lampHours = value;
                OnPropertyChanged("LampHours");
            }
        }

        /// <summary>
        /// Gets or sets the current source.
        /// </summary>
        /// <value>
        /// The current source.
        /// </value>
        public ScriptNumber CurrentSource
        {
            get { return _currentSource; }
            set
            {
                if (Equals(value, _currentSource)) return;
                _currentSource = value;
                OnPropertyChanged("CurrentSource");
            }
        }

        /// <summary>
        /// Gets or sets the picture mode.
        /// </summary>
        /// <value>
        /// The picture mode.
        /// </value>
        public ScriptNumber PictureMode
        {
            get { return _pictureMode; }
            set
            {
                if (Equals(value, _pictureMode)) return;
                _pictureMode = value;
                OnPropertyChanged("PictureMode");
            }
        }

        /// <summary>
        /// Gets or sets the state of the power.
        /// </summary>
        /// <value>
        /// The state of the power.
        /// </value>
        public ScriptBoolean PowerState
        {
            get { return _powerState; }
            set
            {
                if (Equals(value, _powerState)) return;
                _powerState = value;
                OnPropertyChanged("PowerState");
            }
        }

        #endregion Public Properties

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}