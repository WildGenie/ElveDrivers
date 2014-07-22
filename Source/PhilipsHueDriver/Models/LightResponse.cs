using RestSharp.Deserializers;

namespace Elve.Driver.PhilipsHue.Models
{
    internal class LightResponse
    {
        #region Public Properties

        public int Id { get; set; }

        [DeserializeAs(Name = "modelid")]
        public string ModelId { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "swversion")]
        public string SoftwareVersion { get; set; }

        [DeserializeAs(Name = "state")]
        public LightStateResponse State { get; set; }

        [DeserializeAs(Name = "type")]
        public string Type { get; set; }

        #endregion Public Properties
    }
}
