using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Internals
{
    /// <summary>
    /// Retries transient failures: network errors, 408, 429 and 5xx responses.
    /// Follows the retry defaults of Microsoft.Extensions.Http.Resilience: three retries
    /// with exponential backoff from 2s (2s, 4s, 8s), each varied by up to ±25% jitter.
    /// Then returns the last response or rethrows the last network error.
    /// </summary>
    internal sealed class RetryingHttpMessageHandler : DelegatingHandler
    {
        private const int MaxRetries = 3;
        private const double Jitter = 0.25;
        private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(2);
        private static readonly Random SharedRandom = new Random();

        private readonly Func<TimeSpan, CancellationToken, Task> _delay;
        private readonly Func<double> _random;

        public RetryingHttpMessageHandler(HttpMessageHandler innerHandler)
            : this(innerHandler, Task.Delay, NextRandom)
        {
        }

        /// <summary>
        /// Intended for tests: <paramref name="delay"/> replaces waiting, and <paramref name="random"/>
        /// returns a value in [0, 1) that places each delay within the jitter range.
        /// </summary>
        internal RetryingHttpMessageHandler(HttpMessageHandler innerHandler, Func<TimeSpan, CancellationToken, Task> delay, Func<double> random)
            : base(innerHandler)
        {
            _delay = delay;
            _random = random;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            for (var retry = 0; ; retry++)
            {
                HttpResponseMessage response;
                try
                {
                    response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch (HttpRequestException) when (retry < MaxRetries)
                {
                    await _delay(DelayBefore(retry), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                if (retry == MaxRetries || !IsTransient(response.StatusCode))
                {
                    return response;
                }

                response.Dispose();
                await _delay(DelayBefore(retry), cancellationToken).ConfigureAwait(false);
            }
        }

        private static bool IsTransient(HttpStatusCode statusCode)
        {
            return statusCode >= HttpStatusCode.InternalServerError
                || statusCode == HttpStatusCode.RequestTimeout
                || statusCode == (HttpStatusCode)429; // TooManyRequests, not available on netstandard2.0
        }

        private TimeSpan DelayBefore(int retry)
        {
            var exponential = BaseDelay.TotalMilliseconds * Math.Pow(2, retry);
            var jitter = 1 - Jitter + (2 * Jitter * _random());

            return TimeSpan.FromMilliseconds(exponential * jitter);
        }

        private static double NextRandom()
        {
            lock (SharedRandom)
            {
                return SharedRandom.NextDouble();
            }
        }
    }
}
