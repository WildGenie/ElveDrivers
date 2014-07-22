/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodecoreTechnologies.Elve.DriverFramework;
using Elve.Driver.PhilipsHue.Models;
using RestSharp;

namespace Elve.Driver.PhilipsHue.Implementation
{
    /// <summary>
    /// Hue Light Control Service
    /// </summary>
    internal class LightService
    {
        #region Private Fields

        private readonly string _baseUrl;
        private readonly ILogger _logger;
        private readonly string _userName;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LightService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="userName">Name of the user.</param>
        public LightService(ILogger logger, string hostName, string userName)
        {
            _logger = logger;
            _baseUrl = string.Format("http://{0}/api", hostName);
            _userName = "weatheralert";// userName;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets all light status asynchronously.
        /// </summary>
        /// <param name="callback">The success callback.</param>
        /// <param name="failed">The failed callback (optional).</param>
        /// <returns>RestRequestAsyncHandle</returns>
        public RestRequestAsyncHandle GetAllStatusAsync(Action<IEnumerable<LightResponse>> callback, Action<Exception> failed = null)
        {
            var client = new RestClient(_baseUrl) { Timeout = 5000 };
            var getLights = new RestRequest("/{username}/lights", Method.GET);
            getLights.AddUrlSegment("username", _userName);

            _logger.Debug("Hue Driver Lights Service requesting light state");
            return ExecuteGetAllAsync(client, getLights, callback, failed);            
        }

        /// <summary>
        /// Sets the light state asynchronous.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="commmand">The commmand.</param>
        /// <returns>RestRequestAsyncHandle</returns>
        public RestRequestAsyncHandle SetLightStateAsync(int lightId, object commmand)
        {
            var client = new RestClient(_baseUrl);
            var putLightState = new RestRequest("/{username}/lights/{id}/state", Method.PUT);
            putLightState.AddUrlSegment("username", _userName);
            putLightState.AddUrlSegment("id", Convert.ToString(lightId));
            putLightState.RequestFormat = DataFormat.Json;
            putLightState.AddBody(commmand);

            _logger.DebugFormat("Hue Driver Lights Service sending state for light #{0}", lightId);
            return SendAsync(client, putLightState, null, null); 
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Executes the request asynchronously.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The get lights.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="failed">The failed callback.</param>
        /// <returns>RestRequestAsyncHandle</returns>
        private RestRequestAsyncHandle ExecuteGetAllAsync(
            IRestClient client,
            IRestRequest request,
            Action<IEnumerable<LightResponse>> callback,
            Action<Exception> failed)
        {
            return client.ExecuteAsync<Dictionary<string, LightResponse>>(request, restResponse =>
            {
                if (restResponse.ErrorException != null)
                {
                    if (failed != null)
                    {
                        failed.Invoke(restResponse.ErrorException);
                    }
                    else
                    {
                        _logger.ErrorFormat("Hue Driver Lights Service error: {0}", restResponse.ErrorException);
                    }
                }
                else if (restResponse.Content.Length < 10)
                {
                    restResponse.ErrorException =
                        new InvalidDataException("Content length too short: " + restResponse.Content.Length);
                    if (failed != null)
                    {
                        failed.Invoke(restResponse.ErrorException);
                    }
                    else
                    {
                        _logger.ErrorFormat("Hue Driver Lights Service error: {0}", restResponse.ErrorException);
                    }
                }
                else if (callback != null)
                {
                    var lightCollection = restResponse.Data.Select(o =>
                    {
                        o.Value.Id = Convert.ToInt16(o.Key);
                        return o.Value;
                    });
                    callback.Invoke(lightCollection);
                }
                else
                {
                    _logger.DebugFormat("Hue Driver Lights Service result: {0}", restResponse.StatusDescription);
                }
            });
        }

        /// <summary>
        /// Sends the asynchronous REST command.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="failed">The failed callback.</param>
        /// <returns>RestRequestAsyncHandle</returns>
        private RestRequestAsyncHandle SendAsync(
            IRestClient client, 
            IRestRequest request,
            Action<IRestResponse> callback, 
            Action<Exception> failed)
        {
            return client.ExecuteAsync(request, restResponse =>
            {
                if (restResponse.ErrorException != null)
                {
                    if (failed != null)
                    {
                        failed.Invoke(restResponse.ErrorException);
                    }
                    else
                    {
                        _logger.ErrorFormat("Hue Driver Lights Service error: {0}", restResponse.ErrorException);
                    }
                }
                else if (callback != null)
                {
                    callback.Invoke(restResponse);
                }
                else
                {
                    _logger.DebugFormat("Hue Driver Lights Service result: {0}", restResponse.StatusDescription);
                }
            });
        }

        #endregion Private Methods
    }
}
