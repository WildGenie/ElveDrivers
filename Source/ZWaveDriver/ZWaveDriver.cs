/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using CodecoreTechnologies.Elve.DriverFramework;

namespace Elve.Driver.OpenZWave
{
    /// <summary>
    /// Geofancy Presence (GeoFencing) Driver
    /// </summary>
    [Driver(
        "Open Z-Wave",
        "Open Z-Wave Driver.",
        "Jonathan Bradshaw",
        "Lighting and Electrical",
        "Z-Wave",
        "zwave",
        DriverCommunicationPort.Network,
        DriverMultipleInstances.MultiplePerDriverService,
        0,
        1,
        DriverReleaseStages.Development,
        "OpenZWave",
        "http://www.openzwave.com/", 
        null)]
    public class OpenZWaveDriver : CodecoreTechnologies.Elve.DriverFramework.Driver, IDisposable
    {
        #region Public Fields

        #endregion Public Fields

        #region Private Fields

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the driver. This typically sets any class variables and hooks
        /// any event handlers such as SerialPort ReceivedBytes, etc.
        /// </summary>
        /// <param name="configFileData">
        ///     Contains the contents of any configuration files specified in the 
        ///     ConfigurationFileNames property.
        /// </param>
        /// <returns>Initial driver ready status.</returns>
        public override bool StartDriver(Dictionary<string, byte[]> configFileData)
        {
            Logger.DebugFormat("{0} v{1} is starting up",
                DeviceDisplayNameInternal,
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            // Driver isn't ready until we get data so return false
            return false;
        }

        /// <summary>
        /// Stops the driver by unhooking any event handlers and releasing any used resources.
        /// </summary>
        public override void StopDriver()
        {
            Logger.DebugFormat("{0} is stopping", DeviceDisplayNameInternal);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose(bool isDisposing)
        {
        }

        #endregion Protected Methods

        #region Private Methods

        #endregion Private Methods
    }
}
