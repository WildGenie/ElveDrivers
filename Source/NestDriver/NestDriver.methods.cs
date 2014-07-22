/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using CodecoreTechnologies.Elve.DriverFramework.Scripting;

namespace Elve.Driver.Nest
{
    public partial class NestDriver
    {
        #region Public Methods

        [ScriptObjectMethod("Set Thermostat Away Mode", "Sets the away mode of a thermostat.", "Set the away mode of {NAME} thermostat #{PARAM|0|1} to {PARAM|1|0}.")]
        [ScriptObjectMethodParameter("StructureId", "The Id of the structure. (1-16)", 1, 16, "StructureNames")]
        [ScriptObjectMethodParameter("Mode", "The away mode.", new[] { 0.0, 1.0, 2.0 }, new[] { "Home", "Away", "Auto Away" })]
        public void SetThermostatAwayMode(ScriptNumber structureId, ScriptNumber mode)
        {
            var id = _structures[structureId.ToPrimitiveInt32() - 1];
            var state = new[] { "home", "away", "auto-away" }[mode.ToPrimitiveInt32()];
            Logger.DebugFormat("{0} Setting Nest Structure {1} occupancy state to {2}", DriverDisplayNameInternal, id.Name, state);
            _nestController.SetAwayState(id.StructureId, state);
        }

        [ScriptObjectMethod("Set Thermostat Cool Set Point",
            "Sets the cool set point temperature of a thermostat.",
            "Set the cool set point of {NAME} thermostat #{PARAM|0|1} to {PARAM|1|82}.")]
        [ScriptObjectMethodParameter("ThermostatId", "The Id of the thermostat. (1-16)", 1, 16, "ThermostatNames")]
        [ScriptObjectMethodParameter("Value", "The cool set point temperature. (65-85)", 65.0, 85.0)]
        public void SetThermostatCoolSetPoint(ScriptNumber thermostatId, ScriptNumber value)
        {
            var id = _thermostats[thermostatId.ToPrimitiveInt32() - 1];
            Logger.DebugFormat("{0} Setting Nest Thermostat {1} Target High Temperature to {2}", DriverDisplayNameInternal, id.Name, value);
            _nestController.SetTargetTemperatureHighF(id.DeviceId, value.ToPrimitiveInt32());
        }

        [ScriptObjectMethod("Set Thermostat Fan Mode", "Sets a thermostat's fan mode.",
            "Set {NAME} thermostat #{PARAM|0|1} fan mode to {PARAM|1|0}.")]
        [ScriptObjectMethodParameter("ThermostatId", "The Id of the thermostat. (1-16)", 1, 16, "ThermostatNames")]
        [ScriptObjectMethodParameter("Mode", "The fan mode: 0=auto, 1=on.", new[] { 0.0, 1.0 }, new[] { "Auto", "On" })]
        public void SetThermostatFanMode(ScriptNumber thermostatId, ScriptNumber mode)
        {
            var id = _thermostats[thermostatId.ToPrimitiveInt32() - 1];
            Logger.DebugFormat("{0} Setting Nest Thermostat {1} fan time mode to {2}", DriverDisplayNameInternal, id.Name, mode.ToPrimitiveBoolean());
            _nestController.SetFanTimer(id.DeviceId, mode.ToPrimitiveBoolean());
        }

        [ScriptObjectMethod("Set Thermostat Heat Set Point",
            "Sets the heat set point temperature of a thermostat.",
            "Set the heat set point of {NAME} thermostat #{PARAM|0|1} to {PARAM|1|75}.")]
        [ScriptObjectMethodParameter("ThermostatId", "The Id of the thermostat. (1-16)", 1, 16, "ThermostatNames")]
        [ScriptObjectMethodParameter("Value", "The heat set point temperature. (65-85)", 65.0, 85.0)]
        public void SetThermostatHeatSetPoint(ScriptNumber thermostatId, ScriptNumber value)
        {
            var id = _thermostats[thermostatId.ToPrimitiveInt32() - 1];
            Logger.DebugFormat("{0} Setting Nest Thermostat {1} Target Low Temperature to {2}", DriverDisplayNameInternal, id.Name, value);
            _nestController.SetTargetTemperatureLowF(id.DeviceId, value.ToPrimitiveInt32());
        }

        /// <summary>
        /// Not implemented in this driver.
        /// </summary>
        /// <param name="thermostatId"></param>
        /// <param name="hold"></param>
        public void SetThermostatHold(ScriptNumber thermostatId, ScriptBoolean hold)
        {
            // Not implemented in this driver, see implementation using ScriptNumber instead
        }

        [ScriptObjectMethod("Set Thermostat Hold Set Point",
            "Sets the hold set point of a thermostat.",
            "Set the hold set point of {NAME} thermostat #{PARAM|0|1} to {PARAM|1|75}.")]
        [ScriptObjectMethodParameter("ThermostatId", "The Id of the thermostat. (1-16)", 1, 16, "ThermostatNames")]
        [ScriptObjectMethodParameter("Value", "The hold set point temperature. (65-85)", 65.0, 85.0)]
        public void SetThermostatHold(ScriptNumber thermostatId, ScriptNumber value)
        {
            var id = _thermostats[thermostatId.ToPrimitiveInt32() - 1];
            Logger.DebugFormat("{0} Setting Nest Thermostat {1} Target Temperature to {2}", DriverDisplayNameInternal, id.Name, value);
            _nestController.SetTargetTemperatureF(id.DeviceId, value.ToPrimitiveInt32());
        }
        [ScriptObjectMethod("Set Thermostat Mode", "Sets the cooling/heating mode of a thermostat.", "Set the cooling/heating mode of {NAME} thermostat #{PARAM|0|1} to {PARAM|1|0}.")]
        [ScriptObjectMethodParameter("ThermostatId", "The Id of the thermostat. (1-16)", 1, 16, "ThermostatNames")]
        [ScriptObjectMethodParameter("Mode", "The thermostat mode.", new[] { 0.0, 1.0, 2.0, 3.0 }, new[] { "Off", "Heat", "Cool", "Heat-Cool" })]
        public void SetThermostatMode(ScriptNumber thermostatId, ScriptNumber mode)
        {
            var id = _thermostats[thermostatId.ToPrimitiveInt32() - 1];
            var state = new[] {"off", "heat", "cool", "heat-cool"}[mode.ToPrimitiveInt32()];
            Logger.DebugFormat("{0} Setting Nest Thermostat {1} HVAC mode to {2}", DriverDisplayNameInternal, id.Name, state);
            _nestController.SetHvacMode(id.DeviceId, state);
        }

        #endregion Public Methods
    }
}
