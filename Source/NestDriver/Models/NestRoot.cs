using System.Collections.Generic;

namespace Elve.Driver.Nest.Models
{
    internal class NestRoot
    {
        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public NestDevices Devices { get; set; }

        /// <summary>
        /// Gets or sets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public Dictionary<string, NestStructure> Structures { get; set; }
    }
}
