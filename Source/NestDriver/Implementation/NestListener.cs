using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using CodecoreTechnologies.Elve.DriverFramework;
using Elve.Driver.Nest.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace Elve.Driver.Nest.Implementation
{
    internal class NestListener : IDisposable, INotifyPropertyChanged
    {
        #region Protected Fields

        /// <summary>
        /// The base URL
        /// </summary>
        protected const string BaseUrl = "https://developer-api.nest.com/.json";

        #endregion Protected Fields

        #region Private Fields

        private const int BounceTimeout = 1000;
        /// <summary>
        /// The debounce timer used to rate limit property change notifications
        /// </summary>
        private readonly Timer _debounceTimer;

        /// <summary>
        /// The simple Json deserializer
        /// </summary>
        private readonly JsonDeserializer _jsonDeserializer = new JsonDeserializer();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NestListener"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="authToken">The authentication token.</param>
        public NestListener(ILogger logger, string authToken)
        {
            EventStream = new EventSource(string.Format("{0}?auth={1}", BaseUrl, authToken))
            {
                MessageTypes = new[] { "put" }
            };

            EventStream.Message += OnMessageReceived;
            EventStream.StateChange += (sender, args) => OnPropertyChanged("IsConnected");

            _debounceTimer = new Timer(_ => OnPropertyChanged("GraphRoot"), null, -1, -1);
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the current thermostat model.
        /// </summary>
        public NestRoot GraphRoot { get; private set; }

        /// <summary>
        /// Gets the event stream.
        /// </summary>
        /// <value>
        /// The event stream.
        /// </value>
        public EventSource EventStream { get; private set; }

        /// <summary>
        /// Gets the event stream source state.
        /// </summary>
        public bool IsConnected { get { return EventStream.ReadyState == EventSource.EventSourceState.Open; } }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Convenience method to connect Firebase service
        /// </summary>
        public void Connect()
        {
            EventStream.Connect();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (EventStream != null)
            {
                EventStream.Message -= OnMessageReceived;
                EventStream.Dispose();
            }

            if (_debounceTimer != null)
            {
                _debounceTimer.Dispose();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Implementation.EventSource.ServerSentEventArgs"/> instance containing the event data.</param>
        private void OnMessageReceived(object sender, EventSource.ServerSentEventArgs args)
        {
            try
            {
                GraphRoot = _jsonDeserializer.Deserialize<FirebaseEvent<NestRoot>>(
                    new RestResponse { Content = args.Data }).Data;
                // Debounce notifications that are less than 1 second apart
                _debounceTimer.Change(BounceTimeout, -1); 
            }
            catch (SerializationException)
            {
            }
        }

        #endregion Private Methods
    }
}
