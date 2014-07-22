using CodecoreTechnologies.Elve.DriverFramework;
using Elve.Driver.Nest.Models;
using RestSharp;

namespace Elve.Driver.Nest.Implementation
{
    internal class Authenticator
    {
        #region Private Fields

        private const string ClientId = "47252335-f3a3-4050-9b96-95100624eaac";
        private const string ClientSecret = "nNyNbY2PGiwGCbji46amxoCWJ";
        private const string NestAuthApi = "https://api.home.nest.com/oauth2/access_token";
        private readonly ILogger _logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Authenticator(ILogger logger)
        {
            _logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="pinCode">The one-time pin code.</param>
        /// <returns>Access Token Response.</returns>
        public string GetAccessToken(string pinCode)
        {
            var client = new RestClient(NestAuthApi);
            var authRequest = new RestRequest(Method.POST);
            authRequest.AddParameter("code", pinCode, ParameterType.QueryString);
            authRequest.AddParameter("client_id", ClientId, ParameterType.QueryString);
            authRequest.AddParameter("client_secret", ClientSecret, ParameterType.QueryString);
            authRequest.AddParameter("grant_type", "authorization_code", ParameterType.QueryString);

            _logger.Debug("Nest Driver sending authentication request to " + NestAuthApi);
            var response = client.Execute<OAuth2Response>(authRequest);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving Nest authentication response: {0}";
                _logger.ErrorFormat(message, response.ErrorException.GetBaseException().ToString());

                return null;
            }

            _logger.InfoFormat("Nest Driver received token {0} expires in {1} ", response.Data.AccessToken, response.Data.ExpiresIn);

            return response.Data.AccessToken;
        }

        #endregion Public Methods
    }
}
