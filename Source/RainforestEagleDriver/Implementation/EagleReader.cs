/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Elve.Driver.RainforestEagle.Models;
using RestSharp;
using RestSharp.Contrib;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace Elve.Driver.RainforestEagle.Implementation
{
    internal sealed class EagleReader
    {
        #region Private Fields

        private readonly string _baseUrl;
        private readonly string _macId;
        private readonly RestClient _restClient;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EagleReader" /> class.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        public EagleReader(string ipAddress)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            _baseUrl = string.Format("http://{0}/", ipAddress);
            _macId = GetMeterMacId();

            _restClient = new RestClient(_baseUrl) { Timeout = 15000 };
            // JSon responses are returned by the Eagle with content type of "text/html"
            _restClient.AddHandler("text/html", new JsonDeserializer());
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the meter Mac identifier.
        /// </summary>
        public string MacId { get { return _macId; } }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the usage data asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task with UsageData</returns>
        public Task<UsageData> GetUsageDataAsync(CancellationToken cancellationToken)
        {
            return ExecutePostAsync<UsageData>(new LocalCommand
            {
                Name = "get_usage_data",
                MacId = _macId
            }, null, cancellationToken)
            .ContinueWith(t => t.Result.Data, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Gets the day usage history asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task</returns>
        public Task<Dictionary<DateTime, double>> GetDayUsageHistoryAsync(CancellationToken cancellationToken)
        {
            return ExecutePostAsync<Dictionary<string, string>>(new LocalCommand
            {
                Name = "get_historical_data",
                MacId = _macId,
                Period = "Day"
            }, null, cancellationToken).ContinueWith(response => Enumerable
                .Range(0, int.Parse(response.Result.Data["data_size"]))
                .ToDictionary(
                    i => response.Result.Data[@"timestamp[" + i + "]"].ParseJsonDate(CultureInfo.InvariantCulture),
                    i => double.Parse(response.Result.Data[@"value[" + i + "]"])), cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Executes the asynchronous POST request.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="timeout">The optional timeout.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IRestResponse Task.</returns>
        private Task<IRestResponse<T>> ExecutePostAsync<T>(
            object request,
            int? timeout, 
            CancellationToken cancellationToken) where T : new()
        {
            var postRequest = new RestRequest(@"cgi-bin/cgi_manager", Method.POST);
            postRequest.AddBody(request);
            postRequest.RequestFormat = DataFormat.Xml;
            if (timeout.HasValue) postRequest.Timeout = timeout.Value;

            return _restClient.ExecuteTaskAsync<T>(postRequest, cancellationToken);
        }

        /// <summary>
        /// Gets the meter Mac identifier.
        /// </summary>
        /// <returns>Mac identifier.</returns>
        private string GetMeterMacId()
        {
            var httpClient = (HttpWebRequest)WebRequest.Create(_baseUrl);
            httpClient.AllowAutoRedirect = true;
            httpClient.Method = "HEAD";
            using (var response = httpClient.GetResponse())
            {
                var query = response.ResponseUri.Query;
                return HttpUtility.ParseQueryString(query)[0];
            }
        }

        #endregion Private Methods
    }
}
