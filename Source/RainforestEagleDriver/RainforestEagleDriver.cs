/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodecoreTechnologies.Elve.DriverFramework;
using Elve.Driver.RainforestEagle.Implementation;
using Elve.Driver.RainforestEagle.Models;

namespace Elve.Driver.RainforestEagle
{
    /// <summary>
    /// Rainforest Automation Energy Meter Driver.
    /// </summary>
    [Driver(
        "EAGLE Energy Gateway",
        "Rainforest Automation Energy Meter IP Gateway.",
        "Jonathan Bradshaw",
        "Lighting & Electrical",
        "Zigbee",
        "energymeter",
        DriverCommunicationPort.Network,
        DriverMultipleInstances.MultiplePerDriverService,
        1,
        0,
        DriverReleaseStages.Production,
        "Rainforest Automation EAGLE™",
        "http://rainforestautomation.com/rfa-z109-eagle/",
        null
        )]
    public partial class RainforestEagleDriver : CodecoreTechnologies.Elve.DriverFramework.Driver, IDisposable
    {
        #region Private Fields

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private EagleReader _eagleReaderService;

        private string _gatewayIpAddress;

        private int _pollingInterval = 20000;

        private TaskTimer _pollingTimer;

        private PropertyInfo[] _properties = new PropertyInfo[0];

        private UsageData _usageData = new UsageData();

        #endregion Private Fields

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
            Logger.DebugFormat("{0} v{1} is starting up using gateway {2}",
                DeviceDisplayNameInternal,
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3),
                _gatewayIpAddress);

            // Cache the list of bindings to be updated from UsageData POCO
            _properties = typeof(UsageData).GetProperties()
                .Where(p => DriverPropertyBindings.ContainsKey(p.Name))
                .ToArray();

            // Instantiate a service object for reading data from the EAGLE(TM) gateway
            _eagleReaderService = new EagleReader(_gatewayIpAddress);

            // Create a timer object that will initiate the first poll request to the gateway
            _pollingTimer = new TaskTimer(PollforEnergyData, 100, _pollingInterval, _cancellationTokenSource.Token);

            // Driver isn't ready until we get energy data so return false
            return false;
        }

        /// <summary>
        /// Stops the driver by unhooking any event handlers and releasing any used resources.
        /// </summary>
        public override void StopDriver()
        {
            Logger.DebugFormat("{0} is stopping", DeviceDisplayNameInternal);

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;

            if (_pollingTimer != null)
                _pollingTimer.Dispose();

            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Dispose();
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Calculates the demand price.
        /// </summary>
        /// <param name="places">The number of decimal places.</param>
        /// <returns>
        /// Demand price to decimal places.
        /// </returns>
        private double CalculateDemandPrice(byte places = 3)
        {
            return Math.Round(_usageData.Demand * _usageData.Price, places, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Poll for the energy data.
        /// </summary>
        private Task PollforEnergyData(CancellationToken cancellationToken)
        {
            Logger.DebugFormat("{0} is requesting energy data from gateway at {1}", DeviceDisplayNameInternal, _gatewayIpAddress);

            try
            {
                return _eagleReaderService.GetUsageDataAsync(cancellationToken)
                    .ContinueWith(task =>
                    {
                        Logger.DebugFormat("{0} received energy data from gateway in {1} ms",
                            DeviceDisplayNameInternal, _pollingTimer.ElapsedTime.TotalMilliseconds);
                        UpdateDeviceProperties(task.Result);
                        IsReady = true;
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} received an error, and will continue to retry. The error was: {1}",
                    DeviceDisplayNameInternal, ex.GetBaseException().Message);
                IsReady = false;
                throw;
            }
        }

        /// <summary>
        /// Updates the device properties.
        /// </summary>
        /// <param name="usageData">The new usage data.</param>
        private void UpdateDeviceProperties(UsageData usageData)
        {
            if (usageData.UsageTimestamp <= _usageData.UsageTimestamp) return;

            _usageData = usageData;

            foreach (var property in _properties)
            {
                var newValue = property.GetValue(usageData, null);
                if (newValue != null)
                {
                    DevicePropertyChangeNotification(property.Name, newValue);
                }
            }

            DevicePropertyChangeNotification("DemandCost", CalculateDemandPrice());
        }

        #endregion Private Methods
    }
}
