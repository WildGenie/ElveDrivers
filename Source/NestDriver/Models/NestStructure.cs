using System;
using System.Collections.Generic;

namespace Elve.Driver.Nest.Models
{
    internal class NestStructure
    {
        /// <summary>
        /// Gets or sets the structure identifier.
        /// </summary>
        /// <value>
        /// The structure identifier.
        /// </value>
        public string StructureId { get; set; }
        /// <summary>
        /// Gets or sets the thermostats.
        /// </summary>
        /// <value>
        /// The thermostats.
        /// </value>
        public List<string> Thermostats { get; set; }
        /// <summary>
        /// Gets or sets the smoke co alarms.
        /// </summary>
        /// <value>
        /// The smoke co alarms.
        /// </value>
        public List<string> SmokeCoAlarms { get; set; }
        /// <summary>
        /// Gets or sets the away.
        /// </summary>
        /// <value>
        /// The away.
        /// </value>
        public string Away { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }
        /// <summary>
        /// Gets or sets the peak period start time.
        /// </summary>
        /// <value>
        /// The peak period start time.
        /// </value>
        public DateTime PeakPeriodStartTime { get; set; }
        /// <summary>
        /// Gets or sets the peak period end time.
        /// </summary>
        /// <value>
        /// The peak period end time.
        /// </value>
        public DateTime PeakPeriodEndTime { get; set; }
        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public string TimeZone { get; set; }
    }
}