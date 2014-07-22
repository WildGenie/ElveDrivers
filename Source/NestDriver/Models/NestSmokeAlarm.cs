using System;

namespace Elve.Driver.Nest.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal class NestSmokeAlarm
    {
        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public string DeviceId { get; set; }
        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>
        /// The locale.
        /// </value>
        public string Locale { get; set; }
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
        /// Gets or sets the last connection.
        /// </summary>
        /// <value>
        /// The last connection.
        /// </value>
        public DateTime LastConnection { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is online.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is online; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnline { get; set; }
        /// <summary>
        /// Gets or sets the battery health.
        /// </summary>
        /// <value>
        /// The battery health.
        /// </value>
        public string BatteryHealth { get; set; }
        /// <summary>
        /// Gets or sets the state of the co alarm.
        /// </summary>
        /// <value>
        /// The state of the co alarm.
        /// </value>
        public string CoAlarmState { get; set; }
        /// <summary>
        /// Gets or sets the state of the smoke alarm.
        /// </summary>
        /// <value>
        /// The state of the smoke alarm.
        /// </value>
        public string SmokeAlarmState { get; set; }
        /// <summary>
        /// Gets or sets the state of the UI color.
        /// </summary>
        /// <value>
        /// The state of the UI color.
        /// </value>
        public string UiColorState { get; set; }
    }
}