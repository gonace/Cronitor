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
        private readonly Queue<Func<HttpResponseMessage>> _outcomes;

        public ScriptedHttpMessageHandler(params Func<HttpResponseMessage>[] outcomes)
        {
            _outcomes = new Queue<Func<HttpResponseMessage>>(outcomes);
        }

        public int Attempts { get; private set; }

        public static Func<HttpResponseMessage> Status(HttpStatusCode statusCode) =>
            () => new HttpResponseMessage(statusCode) { Content = new StringContent("{}") };

        public static Func<HttpResponseMessage> NetworkError() =>
            () => throw new HttpRequestException("Network error");

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Attempts++;
            var outcome = _outcomes.Count > 1 ? _outcomes.Dequeue() : _outcomes.Peek();

            return Task.FromResult(outcome());
        }
    }
}
