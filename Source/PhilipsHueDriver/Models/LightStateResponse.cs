using System.Collections.Generic;
using Elve.Driver.PhilipsHue.Implementation;
using RestSharp.Deserializers;
using System;

namespace Elve.Driver.PhilipsHue.Models
{
    public class LightStateResponse
    {
        #region Public Properties

        [DeserializeAs(Name = "alert")]
        public string Alert { get; set; }

        [DeserializeAs(Name = "bri")]
        public byte Brightness { get; set; }

        [DeserializeAs(Name = "xy")]
        public List<double> ColorCoordinates { get; set; }

        [DeserializeAs(Name = "colormode")]
        public string ColorMode { get; set; }

        [DeserializeAs(Name = "ct")]
        public int ColorTemperature { get; set; }

        [DeserializeAs(Name = "effect")]
        public string Effect { get; set; }

        [DeserializeAs(Name = "hue")]
        public int Hue { get; set; }

        [DeserializeAs(Name = "reachable")]
        public bool IsReachable { get; set; }

        [DeserializeAs(Name = "on")]
        public bool On { get; set; }
        [DeserializeAs(Name = "sat")]
        public int Saturation { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Convert xy color to hexadecimal.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// Hex color string.
        /// </returns>
        public string AsHexColor(string format = "{0} {1} {2}")
        {
            return HueColorConverter.HexFromXy(ColorCoordinates[0], ColorCoordinates[1], format);
        }

        /// <summary>
        /// To the percent brightness value.
        /// </summary>
        /// <returns>Percent value.</returns>
        public int AsPercent()
        {
            return (int) Math.Floor(this.Brightness/2.55);
        }

        #endregion Public Methods
    }
}
