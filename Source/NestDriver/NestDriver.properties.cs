/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Linq;
using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.DriverInterfaces;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;

namespace Elve.Driver.Nest
{
    public partial class NestDriver : IClimateControlDriver
    {
        #region Public Properties

        [ScriptObjectProperty("Set Points (Away Cooling)",
            "Gets or sets the current away cooling set point temperatures (F).",
            "get the current away cooling set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            "set the current away cooling set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Away Cool Set Point (F) Changed",
            "Occurs when a thermostat cooling away set point (F) changes.")]
        public IScriptArray AwayTemperatureHighF
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.AwayTemperatureHighF), 1); }
        }

        [ScriptObjectProperty("Set Points (Away Heating)",
            "Gets or sets the current away heating set point temperatures (F).",
            "get the current away heating set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            "set the current away heating set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Away Heat Set Point (F) Changed",
            "Occurs when a thermostat heating away set point (F) changes.")]
        public IScriptArray AwayTemperatureLowF
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.AwayTemperatureLowF), 1); }
        }

        [ScriptObjectProperty("Emergency Heat In Use",
            "Gets the current thermostat emergency heat state.",
            "the emergency heat state for {NAME} thermostat #{INDEX|1}",
            null, typeof(ScriptBoolean), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Emergency Heat State Changed",
            "Occurs when a thermostat emergency heat state changes.")]
        public IScriptArray IsUsingEmergencyHeat
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.IsUsingEmergencyHeat), 1, false); }
        }

        // TODO: Needs coding
        public ScriptPagedListCollection PagedListThermostats { get; private set; }

        [ScriptObjectProperty("Location Names",
            "Gets the names of all structures.",
            "the name of {NAME} structure #{INDEX|1}", null)]
        [SupportsDriverPropertyBinding]
        public IScriptArray StructureNames
        {
            get { return new ScriptArrayMarshalByValue(_structures.Select(s => s.Name), 1, false); }
        }

        [ScriptObjectProperty("Set Points (Cooling)",
            "Gets or sets the current cooling set point temperatures (F).",
            "get the current cooling set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            "set the current cooling set point temperature (F) for {NAME} thermostat #{INDEX|1}",
             typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Cool Set Point (F) Changed",
            "Occurs when a thermostat cooling set point (F) changes.")]
        public IScriptArray ThermostatCoolSetPoints
        {
            get
            {
                return new ScriptArrayMarshalByReference(_thermostats.Select(t =>
                    {
                        switch (t.HvacMode)
                        {
                            case "off": return 0;
                            case "heat":
                            case "cool": return 0;
                            default: return t.TargetTemperatureHighF;
                        }
                    }), SetThermostatCoolSetPoint, 1);
            }
        }

        [ScriptObjectProperty("Current Temperatures",
            "Gets the current thermostat temperature (F).",
            "the current temperature (F) for {NAME} thermostat #{INDEX|1}",
            null, typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Current Temperature (F) Changed",
            "Occurs when a thermostat current temperature (F) changes.")]
        public IScriptArray ThermostatCurrentTemperatures
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.AmbientTemperatureF), 1, false); }
        }

        [ScriptObjectProperty("Thermostat Fan Modes",
            "Gets or sets the current thermostat fan timer modes.",
            new[] { 0.0, 1.0 }, new[] { "Auto", "On" },
            "gets the current fan mode for {NAME} thermostat #{INDEX|1}",
            "sets the current fan mode for {NAME} thermostat #{INDEX|1}",
            typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Fan Timer Mode Changed",
            "Occurs when a thermostat's timer fan mode changes.")]
        public IScriptArray ThermostatFanModes
        {
            get { return new ScriptArrayMarshalByReference(_thermostats.Select(t => Convert.ToDouble(t.FanTimerActive)), SetThermostatFanMode, 1); }
        }

        [ScriptObjectProperty("Thermostat Fan Mode Texts",
            "Gets the current thermostat fan modes as displayable text.",
            "the current fan mode text for {NAME} thermostat #{INDEX|1}",
            null, typeof(ScriptString), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding]
        public IScriptArray ThermostatFanModeTexts
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.FanTimerActive ? "On" : "Auto"), 1, false); }
        }

        [ScriptObjectProperty("Set Points (Heating)",
            "Gets or sets the current heat set point temperatures (F).",
            "gets the current heat set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            "sets the current heat set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Heat Set Point (F) Changed",
            "Occurs when a thermostat heating set point (F) changes.")]
        public IScriptArray ThermostatHeatSetPoints
        {
            get
            {
                return new ScriptArrayMarshalByReference(_thermostats.Select(t =>
                    {
                        switch (t.HvacMode)
                        {
                            case "off": return 0;
                            case "heat":
                            case "cool": return 0;
                            default: return t.TargetTemperatureLowF;
                        }
                    }), SetThermostatHeatSetPoint, 1);
            }
        }

        [ScriptObjectProperty("Set Points (Hold Mode)",
            "Gets the current thermostat target set points (F).",
            "gets the current target set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            "sets the current target set point temperature (F) for {NAME} thermostat #{INDEX|1}",
            typeof(ScriptNumber), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Target Set Point (F) Changed",
            "Occurs when a thermostat's target set point (F) changes.")]
        public IScriptArray ThermostatHolds
        {
            get
            {
                return new ScriptArrayMarshalByReference(_thermostats.Select(t =>
                    {
                        switch (t.HvacMode)
                        {
                            case "off": return 0;
                            case "heat":
                            case "cool": return t.TargetTemperatureF;
                            default: return 0;
                        }
                    }), SetThermostatHold, 1);
            }
        }

        [ScriptObjectProperty("Thermostat Connected", 
            "Gets a value indicating if a connection is established with the Nest device.", 
            "a value indicating if the {NAME} is connected", 
            null, typeof(ScriptBoolean), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Connected Status Changed",
            "Occurs when a thermostat connected status changes.")]
        public IScriptArray ThermostatIsOnLine
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.IsOnline), 1, false); }
        }

        [ScriptObjectProperty("Thermostat Last Update",
            "Gets the last connection from the thermostat.",
            "gets the last connection for {NAME} thermostat #{INDEX|1}",
            null,
            typeof(ScriptDateTime), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding]
        public IScriptArray ThermostatLastConnection
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.LastConnection), 1, false); }
        }

        [ScriptObjectProperty("Thermostat Leaf Displayed",
            "Gets the energy saving (leaf) temperature selected.",
            "the current leaf state for {NAME} thermostat #{INDEX|1}",
            null, typeof(ScriptBoolean), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Leaf State Changed", 
            "Occurs when a thermostat leaf state changes.")]
        public IScriptArray ThermostatLeafs
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.HasLeaf), 1, false); }
        }

        [ScriptObjectProperty("Thermostat Long Names",
            "Gets the long names of all thermostats.",
            "the long name of {NAME} thermostat #{INDEX|1}", null)]
        [SupportsDriverPropertyBinding]
        public IScriptArray ThermostatLongNames
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.NameLong), 1, false); }
        }

        [ScriptObjectProperty("Thermostat Modes",
            "Gets or sets the current thermostat modes.",
            new[] { 0.0, 1.0, 2.0, 3.0 }, new[] { "Off", "Heat", "Cool", "Heat-Cool" },
            "gets the current mode for {NAME} thermostat #{INDEX|1}",
            "sets the current mode for {NAME} thermostat #{INDEX|1}",
            typeof(ScriptString), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding("Thermostat Mode Changed", "Occurs when a thermostat mode changes.")]
        public IScriptArray ThermostatModes
        {
            get { return new ScriptArrayMarshalByReference(_thermostats.Select(t =>
            {
                switch (t.HvacMode)
                {
                    case "off": return 0.0;
                    case "heat": return 1.0;
                    case "cool": return 2.0;
                    case "heat-cool": return 3.0;
                    default: return 4.0;
                }
            }), SetThermostatMode, 1); }
        }

        [ScriptObjectProperty("Location Away Modes",
            "Gets the current structure away modes.",
            new[] { 0.0, 1.0, 2.0 }, new[] { "Home", "Away", "Auto-Away" },
            "gets the current away mode for {NAME} structure #{INDEX|1}",
            "sets the current away mode for {NAME} structure #{INDEX|1}",
            typeof(ScriptNumber), 1, 16, "StructureNames")]
        [SupportsDriverPropertyBinding("Away Mode Changed", "Occurs when the away mode changes.")]
        public IScriptArray ThermostatModeTexts
        {
            get { return new ScriptArrayMarshalByReference(_structures.Select(s =>
            {
                switch (s.Away)
                {
                    case "home": return 0;
                    case "away": return 1;
                    default: return 2;
                }
            }), SetThermostatAwayMode, 1); }
        }

        [ScriptObjectProperty("Thermostat Names",
            "Gets the names of all thermostats.",
            "the name of {NAME} thermostat #{INDEX|1}", null)]
        [SupportsDriverPropertyBinding]
        public IScriptArray ThermostatNames
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.Name), 1, false); }
        }

        [ScriptObjectProperty("Thermostat Software Versions",
            "Gets the software version from the thermostat.",
            "gets the software version for {NAME} thermostat #{INDEX|1}",
            null,
            typeof(ScriptString), 1, 16, "ThermostatNames")]
        [SupportsDriverPropertyBinding]
        public IScriptArray ThermostatSofwareVersions
        {
            get { return new ScriptArrayMarshalByValue(_thermostats.Select(t => t.SoftwareVersion), 1, false); }
        }

        #endregion Public Properties
    }
}
