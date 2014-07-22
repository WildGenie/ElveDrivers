using System.Collections.Generic;

namespace Elve.Driver.Nest.Models
{
    internal class NestDevices
    {
        /// <summary>
        /// Gets or sets the thermostats.
        /// </summary>
        /// <value>
        /// The thermostats.
        /// </value>
        public Dictionary<string, NestThermostat> Thermostats { get; set; }

        /// <summary>
        /// Gets or sets the smoke co alarms.
        /// </summary>
        /// <value>
        /// The smoke co alarms.
        /// </value>
        public Dictionary<string, NestSmokeAlarm> SmokeCoAlarms { get; set; }
    }
}