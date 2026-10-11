using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Tests.Helpers
{
    /// <summary>
    /// Replaces the network with a script: each request gets the next outcome,
    /// and the last outcome repeats once the script runs out.
    /// </summary>
    public class ScriptedHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<Func<CancellationToken, Task<HttpResponseMessage>>> _outcomes;

        public ScriptedHttpMessageHandler(params Func<CancellationToken, Task<HttpResponseMessage>>[] outcomes)
        {
            _outcomes = new Queue<Func<CancellationToken, Task<HttpResponseMessage>>>(outcomes);
        }

        public int Attempts { get; private set; }

        public static Func<CancellationToken, Task<HttpResponseMessage>> Status(HttpStatusCode statusCode) =>
            _ => Task.FromResult(new HttpResponseMessage(statusCode) { Content = new StringContent("{}") });

        public static Func<CancellationToken, Task<HttpResponseMessage>> NetworkError() =>
            _ => throw new HttpRequestException("Network error");

        /// <summary>
        /// Never responds; gives up only when the request is cancelled.
        /// </summary>
        public static Func<CancellationToken, Task<HttpResponseMessage>> Hang() =>
            async cancellationToken =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                throw new InvalidOperationException("Unreachable: the delay only ends by cancellation.");
            };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Attempts++;
            var outcome = _outcomes.Count > 1 ? _outcomes.Dequeue() : _outcomes.Peek();

            return outcome(cancellationToken);
        }
    }
}
