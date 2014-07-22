using System.Collections.Generic;
using System.Linq;
using CodecoreTechnologies.Elve.DriverFramework;
using Elve.Driver.PhilipsHue.Models;
using RestSharp;

namespace Elve.Driver.PhilipsHue.Implementation
{
    internal class BridgeLocator
    {
        #region Private Fields

        private const string NuPnPUrl = "http://www.meethue.com/api/nupnp";
        private readonly ILogger _logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeLocator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public BridgeLocator(ILogger logger)
        {
            _logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Executes the request for the Hue Bridge IP address using MeetHue.
        /// </summary>
        /// <returns></returns>
        public NuPnPResponse Execute()
        {
            var client = new RestClient(NuPnPUrl);
            var get = new RestRequest(Method.GET);

            _logger.Debug("Hue Driver Bridge Locator requesting " + NuPnPUrl);
            var response = client.Execute<List<NuPnPResponse>>(get);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                const string message = "Error retrieving bridge locator response: {0}";
                _logger.ErrorFormat(message, response.ErrorException.GetBaseException().ToString());

                return null;
            }

            return response.Data.FirstOrDefault();
        }

        #endregion Public Methods
    }
}
