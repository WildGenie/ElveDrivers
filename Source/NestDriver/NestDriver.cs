/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using CodecoreTechnologies.Elve.DriverFramework;
using Elve.Driver.Nest.Implementation;
using Elve.Driver.Nest.Models;

namespace Elve.Driver.Nest
{
    /// <summary>
    /// Google Nest Thermostat API Driver.
    /// </summary>
    [Driver(
        "Nest Thermostat",
        "Google Nest Thermostat. Requires API access (instructions go here).",
        "Jonathan Bradshaw",
        "Climate Control",
        "Thermostat",
        "nest",
        DriverCommunicationPort.Network,
        DriverMultipleInstances.NotAllowed,
        0,
        1,
        DriverReleaseStages.Development,
        "Google",
        "http://nest.com/", 
        null)]
    public partial class NestDriver : CodecoreTechnologies.Elve.DriverFramework.Driver
    {
        #region Private Fields

        private const string TestAuth = "c.c2wMeOpxejjOMibE8oGVQdldSDHLMGtsD6eGWmWBiTowRLIC1hI3gZNoE0NRn9I0kR5UwV34W1ESZol9hmVcv2kAwSiHA4mQYpiifX8c5ujQ8AiPeGKK2KNGsiGXGjn7Sm6W95TAvO500f0x";

        private NestController _nestController;
        private NestListener _nestListener;
        private NestStructure[] _structures = new NestStructure[0];
        private NestThermostat[] _thermostats = new NestThermostat[0];

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Starts the driver. This typcially sets any class variables and hooks
        /// any event handlers such as SerialPort ReceivedBytes, etc.
        /// </summary>
        /// <param name="configFileData">
        ///     Contains the contents of any configuration files specified in the 
        ///     ConfigurationFileNames property.
        /// </param>
        /// <returns>Initial driver ready status.</returns>
        public override bool StartDriver(Dictionary<string, byte[]> configFileData)
        {
            // Configure service settings for small request packet performance
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            // Instantiate and Start the thermostat service
            _nestListener = new NestListener(Logger, TestAuth);
            _nestListener.PropertyChanged += NestUpdateHandler;
            _nestListener.Connect();

            _nestController = new NestController(TestAuth);

            // Driver isn't ready until we get nest data so return false
            return false;
        }

        /// <summary>
        /// Stops the driver by unhooking any event handlers and releasing any used resources.
        /// </summary>
        public override void StopDriver()
        {
            if (_nestListener != null)
            {
                _nestListener.PropertyChanged -= NestUpdateHandler;
                _nestListener.Dispose();
                _nestListener = null;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Nests the update handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void NestUpdateHandler(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "IsConnected":
                    if (!_nestListener.IsConnected) IsReady = false;
                    break;

                case "GraphRoot":
                    _thermostats = _nestListener.GraphRoot.Devices.Thermostats.Values.ToArray();
                    _structures = _nestListener.GraphRoot.Structures.Values.ToArray();

                    Logger.DebugFormat("{0} received new data graph from Nest", DriverDisplayNameInternal);

                    // Brute force method to update all the driver fields
                    foreach (var property in DriverPropertyBindings.Keys)
                    {
                        DevicePropertyChangeNotification(property);
                    }

                    IsReady = true;
                    break;
            }
        }

        #endregion Private Methods
    }
}
