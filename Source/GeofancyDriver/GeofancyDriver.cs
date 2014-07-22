/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.DeviceSettingEditors;
using CodecoreTechnologies.Elve.DriverFramework.Extensions;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;
using Mono.Nat;

namespace Elve.Driver.Geofancy
{
    /// <summary>
    ///     Geofancy Presence (GeoFencing) Driver
    /// </summary>
    [Driver(
        "Geofancy Presence",
        "Geofancy iPhone Presence Driver. Provides an HTTP webserver to send Geofancy notifications to.",
        "Jonathan Bradshaw",
        "Presence",
        "Geofence",
        "presence",
        DriverCommunicationPort.Network,
        DriverMultipleInstances.MultiplePerDriverService,
        1,
        0,
        DriverReleaseStages.Test,
        "Geofancy",
        "http://geofancy.com/",
        null)]
    public class GeofancyDriver : CodecoreTechnologies.Elve.DriverFramework.Driver, IDisposable
    {
        #region Public Fields

        [DriverEvent("Location Changed", "Occurs when the location changes (entry or exit).")]
        [DriverEventParameter("Location Id", "Specifies the device location id the rule applies to.", true)] 
        [DriverEventParameter("Device", "Specifies the device the rule applies to.", false)] 
        [DriverEventParameter("Trigger", "Specifies the device trigger the rule applies to.", new[] {"enter", "exit"}, false)] 
        public DriverEvent LocationChanged;

        #endregion Public Fields

        #region Private Fields

        private string _device = string.Empty;

        private bool _enableUpnp = true;

        private HttpListener _httpListener;

        private double _latitude;

        private string _locationId = string.Empty;

        private double _longitude;

        private string _password = "geofancy";

        private int _portNumber = 14570;

        private DateTime _timestamp;

        private string _trigger = string.Empty;

        private INatDevice _uPnpRouter;

        private string _userName = "geofancy";

        #endregion Private Fields

        #region Public Properties

        [ScriptObjectProperty("Device", "Gets the current device.", "the {NAME} device", null)]
        [SupportsDriverPropertyBinding]
        public ScriptString Device
        {
            get { return new ScriptString(_device); }
        }

        [DriverSetting("Enable UPnP", "Attempt to automatically configure UPnP port forwarding.",
            typeof (BooleanDriverSettingEditor), "True", false)]
        public bool EnableUpnp
        {
            set { _enableUpnp = value; }
        }

        [ScriptObjectProperty("Latitude", "Gets the device latitude.", "the {NAME} latitude", null)]
        [SupportsDriverPropertyBinding]
        public ScriptNumber Latitude
        {
            get { return new ScriptNumber(_latitude); }
        }

        [ScriptObjectProperty("Location Id", "Gets the device location Id.", "the {NAME} location Id", null)]
        [SupportsDriverPropertyBinding]
        public ScriptString LocationId
        {
            get { return new ScriptString(_locationId); }
        }

        [ScriptObjectProperty("Longitude", "Gets the device longitude.", "the {NAME} longitude", null)]
        [SupportsDriverPropertyBinding]
        public ScriptNumber Longitude
        {
            get { return new ScriptNumber(_longitude); }
        }

        [DriverSetting("Password", "The allowed password for connections from Geofancy.", "geofancy", false)]
        public string Password
        {
            set { _password = value; }
        }

        [DriverSetting("Port Number", "The port number to listen on for connections from Geofancy.", "14570", false)]
        public int PortNumber
        {
            set { _portNumber = value; }
        }

        [ScriptObjectProperty("Timestamp", "Gets the change timestamp.", "the {NAME} timestamp", null)]
        [SupportsDriverPropertyBinding]
        public ScriptDateTime Timestamp
        {
            get { return new ScriptDateTime(_timestamp); }
        }

        [ScriptObjectProperty("Trigger", "Gets the location change trigger (enter or exit).", "the {NAME} trigger", null
            )]
        [SupportsDriverPropertyBinding]
        public ScriptString Trigger
        {
            get { return new ScriptString(_trigger); }
        }

        [DriverSetting("Username", "The allowed username for connections from Geofancy.", "geofancy", false)]
        public string UserName
        {
            set { _userName = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Starts the driver. This typically sets any class variables and hooks
        ///     any event handlers such as SerialPort ReceivedBytes, etc.
        /// </summary>
        /// <param name="configFileData">
        ///     Contains the contents of any configuration files specified in the
        ///     ConfigurationFileNames property.
        /// </param>
        /// <returns>Initial driver ready status.</returns>
        public override bool StartDriver(Dictionary<string, byte[]> configFileData)
        {
            Logger.DebugFormat("{0} v{1} is starting up on port {2}.",
                DeviceDisplayNameInternal,
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3),
                _portNumber);

            // Try and autoconfigure port forwarding in the router using UPNP
            if (_enableUpnp)
                SetupPortForwardingAsync();

            // Create the web server
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(string.Format("http://+:{0}/", _portNumber));
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            // Start listening for connections
            StartListeningAsync();

            // Driver isn't ready until we get data so return false
            return false;
        }

        /// <summary>
        ///     Stops the driver by unhooking any event handlers and releasing any used resources.
        /// </summary>
        public override void StopDriver()
        {
            Logger.DebugFormat("{0} is stopping", DeviceDisplayNameInternal);

            if (_httpListener != null)
                _httpListener.Close();

            if (_enableUpnp)
            {
                NatUtility.StopDiscovery();
            }

            if (_uPnpRouter != null)
            {
                Logger.InfoFormat("{0} is removing UPnP forwarding for port {1}", DeviceDisplayNameInternal, _portNumber);
                _uPnpRouter.DeletePortMap(new Mapping(Protocol.Tcp, _portNumber, _portNumber));
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        ///     Authenticates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        protected virtual bool Authenticate(HttpListenerBasicIdentity user)
        {
            return (user.Name == _userName && user.Password == _password);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;

            if (_httpListener != null)
            {
                _httpListener.Close();
                _httpListener = null;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        ///     Processes the HTTP GET or POST request.
        /// </summary>
        /// <param name="request">The request.</param>
        private static NameValueCollection GetQueryString(HttpListenerRequest request)
        {
            switch (request.HttpMethod)
            {
                case "POST":
                    return HttpUtility.ParseQueryString(request.InputStream.ReadAllText());

                case "GET":
                    return request.QueryString;
            }

            return null;
        }

        /// <summary>
        ///     Converts Unix time stamp to date time.
        /// </summary>
        /// <param name="unixTimeStamp">The Unix time stamp.</param>
        /// <returns>The local DateTime.</returns>
        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        ///     Sets up the port forwarding in the background.
        /// </summary>
        private void SetupPortForwardingAsync()
        {
            NatUtility.DeviceFound += (sender, router) =>
            {
                NatUtility.StopDiscovery();
                _uPnpRouter = router.Device;
                Logger.InfoFormat("{0} is creating UPnP external port {1} forwarding on the discovered router.",
                    DeviceDisplayNameInternal, _portNumber);
                _uPnpRouter.CreatePortMap(new Mapping(Protocol.Tcp, _portNumber, _portNumber));
            };

            NatUtility.StartDiscovery();
        }

        /// <summary>
        ///     Starts listening for HTTP requests.
        /// </summary>
        private void StartListeningAsync()
        {
            _httpListener.Start();

            Task.Factory
                .FromAsync<HttpListenerContext>(_httpListener.BeginGetContext, _httpListener.EndGetContext, _httpListener)
                .ContinueWith(task =>
                {
                    // start a new listener for the next request if server is still listening
                    if (_httpListener.IsListening) StartListeningAsync();

                    var ctx = task.Result;
                    if (ctx == null || ctx.Request.RemoteEndPoint == null) return;
                    var sourceIp = ctx.Request.RemoteEndPoint.ToString();

                    Logger.DebugFormat("{0} received HTTP connection from {1}", DeviceDisplayNameInternal, sourceIp);

                    var user = (HttpListenerBasicIdentity) ctx.User.Identity;
                    if (Authenticate(user))
                    {
                        var query = GetQueryString(ctx.Request);
                        if (query != null && query.Count == 6) UpdateDriverProperties(query);
                    }
                    else
                    {
                        Logger.InfoFormat(
                            "{0} received invalid HTTP authentication (name=\"{1}\", password=\"{2}\") from {3}",
                            DeviceDisplayNameInternal, user.Name, user.Password, sourceIp);
                    }

                    ctx.Response.Close();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        ///     Updates the driver properties.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        private void UpdateDriverProperties(NameValueCollection queryString)
        {
            double timestamp;
            _device = queryString["device"];
            _locationId = queryString["id"];
            _trigger = queryString["trigger"];
            double.TryParse(queryString["latitude"], out _latitude);
            double.TryParse(queryString["longitude"], out _longitude);
            if (double.TryParse(queryString["timestamp"], out timestamp))
                _timestamp = UnixTimeStampToDateTime(timestamp);

            Logger.DebugFormat("{0} received {1} trigger at location {2} for device {3}",
                DeviceDisplayNameInternal, _trigger, _locationId, _device);

            // Update the driver properties
            DevicePropertyChangeNotification("Device", _device);
            DevicePropertyChangeNotification("LocationId", _locationId);
            DevicePropertyChangeNotification("Trigger", _trigger);
            DevicePropertyChangeNotification("Latitude", _latitude);
            DevicePropertyChangeNotification("Longitude", _longitude);
            DevicePropertyChangeNotification("Timestamp", _timestamp);

            // Raise the changed event
            RaiseDeviceEvent(LocationChanged);

            IsReady = true;
        }

        #endregion Private Methods
    }
}