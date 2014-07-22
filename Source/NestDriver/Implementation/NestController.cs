using System.Net;
using CodecoreTechnologies.Elve.DriverFramework;
using RestSharp;

namespace Elve.Driver.Nest.Implementation
{
    internal sealed class NestController
    {
        #region Private Fields

        /// <summary>
        ///     The base URL
        /// </summary>
        private const string BaseUrl = "https://developer-api.nest.com";

        /// <summary>
        ///     The rest client
        /// </summary>
        private readonly RestClient _restClient;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NestController" /> class.
        /// </summary>
        /// <param name="authToken">The authentication token.</param>
        public NestController(string authToken)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            _restClient = new RestClient(BaseUrl);
            _restClient.AddDefaultParameter("auth", authToken, ParameterType.QueryString);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Sets the state of the away.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="newState">The new state.</param>
        /// <returns></returns>
        public HttpStatusCode SetAwayState(string id, string newState)
        {
            return SetStructure(id, new {away = newState});
        }

        /// <summary>
        /// Sets the target temperature f.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="tempF">The temporary f.</param>
        /// <returns></returns>
        public HttpStatusCode SetTargetTemperatureF(string id, int tempF)
        {
            return SetThermostat(id, new {target_temperature_f = tempF});
        }

        /// <summary>
        /// Sets the target temperature high f.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="tempF">The temporary f.</param>
        /// <returns></returns>
        public HttpStatusCode SetTargetTemperatureHighF(string id, double tempF)
        {
            return SetThermostat(id, new {target_temperature_high_f = tempF});
        }

        /// <summary>
        /// Sets the target temperature low f.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="tempF">The temporary f.</param>
        /// <returns></returns>
        public HttpStatusCode SetTargetTemperatureLowF(string id, double tempF)
        {
            return SetThermostat(id, new {target_temperature_low_f = tempF});
        }

        /// <summary>
        /// Sets the fan timer.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <returns></returns>
        public HttpStatusCode SetFanTimer(string id, bool state)
        {
            return SetThermostat(id, new {fan_timer_active = state});
        }

        /// <summary>
        /// Sets the hvac mode.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public HttpStatusCode SetHvacMode(string id, string state)
        {
            return SetThermostat(id, new {hvac_mode = state});
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Sets the smoke alarm.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private HttpStatusCode SetSmokeAlarm(string id, object state)
        {
            return Execute(new RestRequest("/devices/smoke_co_alarms/{id}.json", Method.PUT), id, state).StatusCode;
        }

        /// <summary>
        /// Sets the structure.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private HttpStatusCode SetStructure(string id, object state)
        {
            return Execute(new RestRequest("/structures/{id}.json", Method.PUT), id, state).StatusCode;
        }

        /// <summary>
        /// Sets the thermostat.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private HttpStatusCode SetThermostat(string id, object state)
        {
            return Execute(new RestRequest("/devices/thermostats/{id}.json", Method.PUT), id, state).StatusCode;
        }

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private IRestResponse Execute(IRestRequest request, string id, object state)
        {
            request.AddUrlSegment("id", id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(state);
            return _restClient.Execute(request);
        }

        #endregion Private Methods
    }
}