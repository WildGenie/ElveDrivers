using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Elve.Driver.RainforestEagle.Implementation
{
    internal static class RestSharpExtensions
    {
        /// <summary>
        /// Convert a <see cref="ResponseStatus" /> to a <see cref="WebException" /> instance.
        /// </summary>
        /// <param name="responseStatus">The response status.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">responseStatus</exception>
        public static WebException ToWebException(this ResponseStatus responseStatus)
        {
            switch (responseStatus)
            {
                case ResponseStatus.None:
                    return new WebException("The request could not be processed.", WebExceptionStatus.ServerProtocolViolation);
                case ResponseStatus.Error:
                    return new WebException("An error occurred while processing the request.", WebExceptionStatus.ServerProtocolViolation);
                case ResponseStatus.TimedOut:
                    return new WebException("The request timed-out.", WebExceptionStatus.Timeout);
                case ResponseStatus.Aborted:
                    return new WebException("The request was aborted.", WebExceptionStatus.Timeout);
                default:
                    throw new ArgumentOutOfRangeException("responseStatus");
            }
        }

        /// <summary>
        /// Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="client">The client.</param>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">request</exception>
        public static Task<IRestResponse<T>> ExecuteTaskAsync<T>(this IRestClient client, IRestRequest request, CancellationToken token)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();

            try
            {
                var async = client.ExecuteAsync<T>(request, (response, _) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                    else if (response.ErrorException != null)
                    {
                        taskCompletionSource.TrySetException(response.ErrorException);
                    }
                    else if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        taskCompletionSource.TrySetException(response.ResponseStatus.ToWebException());
                    }
                    else
                    {
                        taskCompletionSource.TrySetResult(response);
                    }
                });

                token.Register(() =>
                {
                    async.Abort();
                    taskCompletionSource.TrySetCanceled();
                });
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }

            return taskCompletionSource.Task;
        }
    }
}
