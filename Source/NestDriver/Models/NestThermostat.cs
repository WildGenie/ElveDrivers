using System;

namespace Elve.Driver.Nest.Models
{
    internal class NestThermostat
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ambient temperature c.
        /// </summary>
        /// <value>
        /// The ambient temperature c.
        /// </value>
        public double AmbientTemperatureC { get; set; }

        /// <summary>
        /// Gets or sets the ambient temperature f.
        /// </summary>
        /// <value>
        /// The ambient temperature f.
        /// </value>
        public int AmbientTemperatureF { get; set; }

        /// <summary>
        /// Gets or sets the away temperature high c.
        /// </summary>
        /// <value>
        /// The away temperature high c.
        /// </value>
        public double AwayTemperatureHighC { get; set; }

        /// <summary>
        /// Gets or sets the away temperature high f.
        /// </summary>
        /// <value>
        /// The away temperature high f.
        /// </value>
        public int AwayTemperatureHighF { get; set; }

        /// <summary>
        /// Gets or sets the away temperature low c.
        /// </summary>
        /// <value>
        /// The away temperature low c.
        /// </value>
        public double AwayTemperatureLowC { get; set; }

        /// <summary>
        /// Gets or sets the away temperature low f.
        /// </summary>
        /// <value>
        /// The away temperature low f.
        /// </value>
        public int AwayTemperatureLowF { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can cool.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can cool; otherwise, <c>false</c>.
        /// </value>
        public bool CanCool { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can heat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can heat; otherwise, <c>false</c>.
        /// </value>
        public bool CanHeat { get; set; }

        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [fan timer active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fan timer active]; otherwise, <c>false</c>.
        /// </value>
        public bool FanTimerActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has fan.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has fan; otherwise, <c>false</c>.
        /// </value>
        public bool HasFan { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has leaf.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has leaf; otherwise, <c>false</c>.
        /// </value>
        public bool HasLeaf { get; set; }

        /// <summary>
        /// Gets or sets the hvac mode.
        /// </summary>
        /// <value>
        /// The hvac mode.
        /// </value>
        public string HvacMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is online.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is online; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is using emergency heat.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is using emergency heat; otherwise, <c>false</c>.
        /// </value>
        public bool IsUsingEmergencyHeat { get; set; }

        /// <summary>
        /// Gets or sets the last connection.
        /// </summary>
        /// <value>
        /// The last connection.
        /// </value>
        public DateTime LastConnection { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>
        /// The locale.
        /// </value>
        public string Locale { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name long.
        /// </summary>
        /// <value>
        /// The name long.
        /// </value>
        public string NameLong { get; set; }

        /// <summary>
        /// Gets or sets the software version.
        /// </summary>
        /// <value>
        /// The software version.
        /// </value>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the structure identifier.
        /// </summary>
        /// <value>
        /// The structure identifier.
        /// </value>
        public string StructureId { get; set; }

        /// <summary>
        /// Gets or sets the target temperature c.
        /// </summary>
        /// <value>
        /// The target temperature c.
        /// </value>
        public double TargetTemperatureC { get; set; }

        /// <summary>
        /// Gets or sets the target temperature f.
        /// </summary>
        /// <value>
        /// The target temperature f.
        /// </value>
        public int TargetTemperatureF { get; set; }

        /// <summary>
        /// Gets or sets the target temperature high c.
        /// </summary>
        /// <value>
        /// The target temperature high c.
        /// </value>
        public double TargetTemperatureHighC { get; set; }

        /// <summary>
        /// Gets or sets the target temperature high f.
        /// </summary>
        /// <value>
        /// The target temperature high f.
        /// </value>
        public int TargetTemperatureHighF { get; set; }

        /// <summary>
        /// Gets or sets the target temperature low c.
        /// </summary>
        /// <value>
        /// The target temperature low c.
        /// </value>
        public double TargetTemperatureLowC { get; set; }

        /// <summary>
        /// Gets or sets the target temperature low f.
        /// </summary>
        /// <value>
        /// The target temperature low f.
        /// </value>
        public int TargetTemperatureLowF { get; set; }

        /// <summary>
        /// Gets or sets the temperature scale.
        /// </summary>
        /// <value>
        /// The temperature scale.
        /// </value>
        public string TemperatureScale { get; set; }

        #endregion Public Properties
        #region Public Methods

        /// <summary>
        /// Gets the ambient temperature.
        /// </summary>
        /// <returns>String</returns>
        public string GetAmbientTemperature()
        {
            if (TemperatureScale == "F") return Convert.ToString(AmbientTemperatureF);
            if (TemperatureScale == "C") return Convert.ToString(AmbientTemperatureC);

            return string.Empty;
        }

        /// <summary>
        /// Gets the target temperature or temperature range.
        /// </summary>
        /// <returns>String</returns>
        public string GetTargetTemperature()
        {
            if (HvacMode == "range")
            {
                if (TemperatureScale == "F")
                    return string.Format("{0} - {1}", TargetTemperatureLowF, TargetTemperatureHighF);
                if (TemperatureScale == "C")
                    return string.Format("{0} - {1}", TargetTemperatureLowC, TargetTemperatureHighC);
            }
            else
            {
                if (TemperatureScale == "F") return Convert.ToString(TargetTemperatureF);
                if (TemperatureScale == "C") return Convert.ToString(TargetTemperatureC);
            }

            return string.Empty;
        }

        #endregion Public Methods
    }
}
