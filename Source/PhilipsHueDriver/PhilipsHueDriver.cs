/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.DriverInterfaces;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;
using Elve.Driver.PhilipsHue.Implementation;
using Elve.Driver.PhilipsHue.Models;

namespace Elve.Driver.PhilipsHue
{
    /// <summary>
    /// Philips Hue IP Bridge Driver.
    /// </summary>
    [Driver(
        "Philips Hue",
        "Philips Hue IP Bridge.",
        "Jonathan Bradshaw",
        "Lighting & Electrical",
        "Zigbee",
        "hue",
        DriverCommunicationPort.Network,
        DriverMultipleInstances.MultiplePerDriverService,
        0,
        1,
        DriverReleaseStages.Development,
        "Philips Hue",
        "http://meethue.com/",
        null)]
    public class PhilipsHueDriver : CodecoreTechnologies.Elve.DriverFramework.Driver, ILightingAndElectricalDriver
    {
        #region Private Fields

        private string _bridgeIpAddress;
        private string _bridgeUserName;
        private Timer _lightPollingTimer;
        private List<LightResponse> _lights = new List<LightResponse>();
        private LightService _lightService;
        private int _pollingInterval = 5000;

        #endregion Private Fields

        #region Public Properties

        [DriverSetting("HostName", "The host name or ip address of the Philips Hue Bridge (if blank will attempt autodetection).", null, false)]
        public string HostNameSetting
        {
            set { _bridgeIpAddress = value; }
        }

        [ScriptObjectProperty("Light Colors",
            "Gets the color of the lights.",
            "the {NAME} color for light #{INDEX|0}",
            "Set {NAME} color #{INDEX|0} to {VALUE|0}.",
            typeof(ScriptString), 1, 255, "LightNames")]
        [SupportsDriverPropertyBinding("Light Color Changed", "Occurs when the light color changes.")]
        public IScriptArray LightColors
        {
            get
            {
                return new ScriptArrayMarshalByReference(_lights.Select(l => l.State.AsHexColor()).ToArray(), SetLightColor, 1);
            }
        }

        [ScriptObjectProperty("Light Levels",
            "Gets the percent on level of the lights. 0-99", 0, 99,
            "the {NAME} light level for light #{INDEX|0}",
            "Set {NAME} light #{INDEX|0} to {VALUE|0}.",
            typeof(ScriptNumber), 1, 255, "LightNames")]
        [SupportsDriverPropertyBinding("Light Level Changed", "Occurs when the light level changes.")]
        public IScriptArray LightLevels
        {
            get
            {
                return new ScriptArrayMarshalByReference(_lights.Select(l => l.State.AsPercent()).ToArray(), SetLightLevel, 1);
            }
        }

        [ScriptObjectProperty("Light Names",
            "Gets or sets the names of the lights.",
            "the name of {NAME} light #{INDEX|0}")]
        [SupportsDriverPropertyBinding("Light Name Changed", "Occurs when the light name changes.")]
        public IScriptArray LightNames
        {
            get
            {
                return new ScriptArrayMarshalByValue(_lights.Select(l => l.Name).ToArray(), 1);
            }
        }

        [ScriptObjectProperty("Light On/Offs",
            "Gets or sets the light to on state or off.",
            "the {NAME} on/off state for light #{INDEX|0}",
            "Set {NAME} light #{INDEX|0} to {VALUE|true|on|off}.",
            typeof(ScriptBoolean), 1, 255, "LightNames")]
        [SupportsDriverPropertyBinding("Light On/Off State Changed", "Occurs when the light changes from on to off or off to on.")]
        public IScriptArray LightOnOffs
        {
            get
            {
                return new ScriptArrayMarshalByReference(_lights.Select(l => l.State.On).ToArray(),
                    delegate(ScriptNumber index, ScriptBoolean value)
                    {
                        if (value) { TurnOnLight(index); } else { TurnOffLight(index); }
                    }, 1);
            }
        }

        [ScriptObjectProperty("Light Reachability",
            "Gets the light reachability state.",
            "the {NAME} reachability state for light #{INDEX|0}", null,
            typeof(ScriptBoolean), 1, 255, "LightNames")]
        [SupportsDriverPropertyBinding("Light Reachability State Changed", "Occurs when the light reachability state changes.")]
        public IScriptArray LightReachability
        {
            get
            {
                return new ScriptArrayMarshalByValue(_lights.Select(l => l.State.IsReachable).ToArray(), 1);
            }
        }

        [ScriptObjectProperty("Paged List Lights",
            "Provides the list of lights to be shown in a Touch Screen Interface's Paged List control.")]
        [SupportsDriverPropertyBinding]
        public ScriptPagedListCollection PagedListLights
        {
            get
            {
                return new ScriptPagedListCollection(
                    _lights.Select(light => new ScriptPagedListItem(
                        light.Name,
                        light.State.AsPercent() + "%",
                        new ScriptExpandoObject(new Dictionary<string, IScriptObject>
                        {
                            {"ID", new ScriptNumber(light.Id)},
                            {"IsReachable", new ScriptBoolean(light.State.IsReachable)}
                        })
                    ))
                );
            }
        }

        [DriverSetting("Poll Interval", "The interval (in seconds) between polls for current lamp state.", 1, 60, "5", false)]
        public int PollingIntervalSetting
        {
            set { _pollingInterval = value * 1000; }
        }

        [DriverSetting("UserName", "The name to use for authentication.", "ElveHueDriver", false)]
        public string UserNameSetting
        {
            set { _bridgeUserName = value; }
        }

        #endregion Public Properties

        #region Public Methods

        [ScriptObjectMethod("Send Raw Command", "Sends a command that may otherwise not be supported by the driver.", "Send the raw {NAME} command {PARAM|0|command data}.")]
        [ScriptObjectMethodParameter("Command", "The command string to send.  The driver will append the command with a carriage-return <CR>.")]
        public void SendRawCommand(ScriptString command)
        {
            throw new NotImplementedException();
        }

        [ScriptObjectMethod("Set light color", "Sets the specified light's color to the specified RGB value.", "Set {NAME} light #{PARAM|0|2} to {PARAM|1|99}%.")]
        [ScriptObjectMethodParameter("LightID", "The id of the light.", 1, 255, "LightNames")]
        [ScriptObjectMethodParameter("Color", "The hex RGB color to set the light to.", ">AA AA AA")]
        public void SetLightColor(ScriptNumber lightId, ScriptString value)
        {
            Logger.InfoFormat("Setting light #{0} color to '{1}'", lightId, value);
            var cgPoint = HueColorConverter.XyFromColor(value.ToPrimitiveString().Replace(" ", ""));
            _lightService.SetLightStateAsync(lightId.ToPrimitiveInt32(), new
            {
                on = true,  // cannot modify color when light is off
                xy = new[] { cgPoint.x, cgPoint.y }
            });
        }

        [ScriptObjectMethod("Set light level", "Sets the specified light's level to the specified percent.", "Set {NAME} light #{PARAM|0|2} to {PARAM|1|99}%.")]
        [ScriptObjectMethodParameter("LightID", "The id of the light.", 1, 255, "LightNames")]
        [ScriptObjectMethodParameter("PercentOn", "The percent level to set the light to. Valid values: 0 to 99 where 0 is typically off and 99 is fully on.", 0, 99)]
        public void SetLightLevel(ScriptNumber lightId, ScriptNumber percentOn)
        {
            Logger.InfoFormat("Setting light #{0} level to '{1}'", lightId, percentOn);
            var value = Math.Ceiling(percentOn.ToPrimitiveDouble() * 2.55);
            _lightService.SetLightStateAsync(lightId.ToPrimitiveInt32(), new
            {
                on = true,  // cannot modify brightness when light is off
                bri = value
            });
        }

        // Don't include script build text parameter since the other method with the same name will work for that. This method will be used only from the scripting language.
        [ScriptObjectMethod("Set light level by Name", "Sets the specified light's % level to the specified percent.")]
        [ScriptObjectMethodParameter("Name", "The name of the light.")]
        [ScriptObjectMethodParameter("PercentOn", "The brightness percent level to set the device to. (0-99)", 0, 99)]
        public void SetLightLevelByName(ScriptString name, ScriptNumber percentOn)
        {
            SetLightLevel(FindDeviceName(name), percentOn);
        }

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

            // If bridge IP is blank then locate it using MeetHue service
            ConfigureHueBridgeIp();

            // Instantiate a service object for reading and writing data to the Hue gateway
            _lightService = new LightService(Logger, _bridgeIpAddress, _bridgeUserName);

            // Create a timer object that will initiate a poll request to the gateway
            _lightPollingTimer = new Timer
            {
                AutoReset = false
            };
            _lightPollingTimer.Elapsed += PollHueGatewayState;

            // Start the timer to schedule the next poll
            _lightPollingTimer.Start();

            // Driver isn't ready until we get lighting data so return false
            return false;
        }
        /// <summary>
        /// Stops the driver by unhooking any event handlers and releasing any used resources.
        /// </summary>
        public override void StopDriver()
        {
            if (_lightPollingTimer != null)
            {
                _lightPollingTimer.Dispose();
                _lightPollingTimer = null;
            }
        }

        [ScriptObjectMethod("Turn off all lights", "Turns all lights off.", "Turn off all {NAME} lights.")]
        public void TurnOffAllLights()
        {
            throw new NotImplementedException();
        }

        [ScriptObjectMethod("Turn off light", "Turns off the specified light.", "Turn off {NAME} light #{PARAM|0|2}.")]
        [ScriptObjectMethodParameter("LightID", "The id of the light.", 1, 255, "LightNames")]
        public void TurnOffLight(ScriptNumber lightId)
        {
            Logger.InfoFormat("Setting light #{0} to 'off'", lightId);
            _lightService.SetLightStateAsync(lightId.ToPrimitiveInt32(), new { on = false });
        }

        // Don't include script build text parameter since the other method with the same name will work for that. This method will be used only from the scripting language.
        [ScriptObjectMethod("Turn off light by Name", "Turns off the specified light.")]
        [ScriptObjectMethodParameter("Name", "The name of the light.")]
        public void TurnOffLightByName(ScriptString name)
        {
            TurnOffLight(FindDeviceName(name));
        }

        [ScriptObjectMethod("Turn on all lights", "Turns all lights on.", "Turn on all {NAME} lights.")]
        public void TurnOnAllLights()
        {
            throw new NotImplementedException();
        }

        [ScriptObjectMethod("Turn on light", "Turns on the specified light.", "Turn on {NAME} light #{PARAM|0|2}.")]
        [ScriptObjectMethodParameter("LightID", "The id of the light.", 1, 255, "LightNames")]
        public void TurnOnLight(ScriptNumber lightId)
        {
            Logger.InfoFormat("Setting light #{0} to 'on'", lightId);
            _lightService.SetLightStateAsync(lightId.ToPrimitiveInt32(), new { on = true });
        }
        // Don't include script build text parameter since the other method with the same name will work for that. This method will be used only from the scripting language.
        [ScriptObjectMethod("Turn on light by Name", "Turns on the specified light.")]
        [ScriptObjectMethodParameter("Name", "The name of the light.")]
        public void TurnOnLightByName(ScriptString name)
        {
            TurnOnLight(FindDeviceName(name));
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Configures the bridge ip.
        /// </summary>
        private void ConfigureHueBridgeIp()
        {
            if (!string.IsNullOrEmpty(_bridgeIpAddress)) return;

            Logger.Debug("Hue Driver querying for bridge internal IP address");
            var hueHub = new BridgeLocator(Logger).Execute();
            if (hueHub == null)
            {
                Logger.Fatal("Hue Driver unable to get bridge address.");
            }
            else
            {
                _bridgeIpAddress = hueHub.InternalIpAddress;
                Logger.InfoFormat("Hue Driver bridge internal IP address is {0}", _bridgeIpAddress);
            }
        }

        /// <summary>
        /// Finds the name of the device.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private ScriptNumber FindDeviceName(string name)
        {
            var light = _lights.FirstOrDefault(l => l.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (light != null)
            {
                return new ScriptNumber(light.Id);
            }

            throw new Exception("Light " + name + " not found");
        }

        /// <summary>
        /// Polls the state of the hue gateway.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="elapsedEventArgs">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void PollHueGatewayState(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _lightService.GetAllStatusAsync(data =>
            {
                if (_lightPollingTimer == null) return;

                Logger.Debug("Hue Driver received light data response from bridge");
                UpdateLightState(data.ToList());

                IsReady = true;
                _lightPollingTimer.Interval = _pollingInterval;
                _lightPollingTimer.Start();
            }, exception =>
            {
                if (_lightPollingTimer == null) return;

                IsReady = false;
                Logger.ErrorFormat("Hue Driver received error response: {0}", exception.GetBaseException().Message);
                _lightPollingTimer.Interval = Math.Min(_lightPollingTimer.Interval * 1.5, 60000);
                _lightPollingTimer.Start();
            });
        }

        /// <summary>
        /// Updates the state of the light.
        /// </summary>
        /// <param name="lights">The new lights state.</param>
        private void UpdateLightState(List<LightResponse> lights)
        {
            _lights = lights;

            foreach (var light in lights)
            {
                Logger.DebugFormat("Hue Driver is updating light #{0} ({1}) values", light.Id, light.Name);
                DevicePropertyChangeNotification("LightLevels", light.Id, light.State.AsPercent());
                DevicePropertyChangeNotification("LightNames", light.Id, light.Name);
                DevicePropertyChangeNotification("LightOnOffs", light.Id, light.State.On);
                DevicePropertyChangeNotification("LightReachability", light.Id, light.State.IsReachable);
                DevicePropertyChangeNotification("LightColors", light.Id, light.State.AsHexColor());
            }
        }

        #endregion Private Methods
    }
}
