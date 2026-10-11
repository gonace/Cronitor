using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Cronitor.Tests.Helpers.ScriptedHttpMessageHandler;

namespace Cronitor.Tests
{
    public class RetryingHttpMessageHandlerTests
    {
        private const string Url = "https://cronitor.link/p/apiKey/monitorKey/run";

        private readonly List<TimeSpan> _delays = new List<TimeSpan>();

        [Fact]
        public async Task ShouldNotRetryASuccessfulRequest()
        {
            var network = new ScriptedHttpMessageHandler(Status(HttpStatusCode.OK));

            var response = await SendAsync(network);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, network.Attempts);
            Assert.Empty(_delays);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData((HttpStatusCode)429)]
        public async Task ShouldRetryATransientStatusCode(HttpStatusCode statusCode)
        {
            var network = new ScriptedHttpMessageHandler(Status(statusCode), Status(HttpStatusCode.OK));

            var response = await SendAsync(network);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, network.Attempts);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task ShouldNotRetryAClientError(HttpStatusCode statusCode)
        {
            var network = new ScriptedHttpMessageHandler(Status(statusCode));

            var response = await SendAsync(network);

            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(1, network.Attempts);
        }

        [Fact]
        public async Task ShouldRetryANetworkError()
        {
            var network = new ScriptedHttpMessageHandler(NetworkError(), Status(HttpStatusCode.OK));

            var response = await SendAsync(network);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, network.Attempts);
        }

        [Fact]
        public async Task ShouldReturnTheLastResponseAfterThreeRetries()
        {
            var network = new ScriptedHttpMessageHandler(Status(HttpStatusCode.ServiceUnavailable));

            var response = await SendAsync(network);

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.Equal(4, network.Attempts);
        }

        [Fact]
        public async Task ShouldThrowTheNetworkErrorAfterThreeRetries()
        {
            var network = new ScriptedHttpMessageHandler(NetworkError());

            await Assert.ThrowsAsync<HttpRequestException>(() => SendAsync(network));

            Assert.Equal(4, network.Attempts);
        }

        [Fact]
        public async Task ShouldBackOffExponentially()
        {
            var network = new ScriptedHttpMessageHandler(Status(HttpStatusCode.ServiceUnavailable));

            await SendAsync(network, random: 0.5);

            Assert.Equal(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8) }, _delays);
        }

        [Fact]
        public async Task ShouldShortenTheDelayByAtMostAQuarter()
        {
            var network = new ScriptedHttpMessageHandler(Status(HttpStatusCode.ServiceUnavailable), Status(HttpStatusCode.OK));

            await SendAsync(network, random: 0.0);

            Assert.Equal(TimeSpan.FromSeconds(1.5), Assert.Single(_delays));
        }

        [Fact]
        public async Task ShouldLengthenTheDelayByAtMostAQuarter()
        {
            var network = new ScriptedHttpMessageHandler(Status(HttpStatusCode.ServiceUnavailable), Status(HttpStatusCode.OK));

            await SendAsync(network, random: 1.0);

            Assert.Equal(TimeSpan.FromSeconds(2.5), Assert.Single(_delays));
        }

        [Fact]
        public async Task ShouldNotRetryWhenTheCallerCancels()
        {
            var network = new ScriptedHttpMessageHandler(Status(HttpStatusCode.ServiceUnavailable));
            var cancellation = new CancellationTokenSource();
            var handler = new RetryingHttpMessageHandler(network, (delay, token) =>
            {
                cancellation.Cancel();
                return Task.Delay(delay, token);
            }, () => 0.5);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                new System.Net.Http.HttpClient(handler).GetAsync(Url, cancellation.Token));

            Assert.Equal(1, network.Attempts);
        }

        private Task<HttpResponseMessage> SendAsync(HttpMessageHandler network, double random = 0.5)
        {
            var handler = new RetryingHttpMessageHandler(network, RecordDelay, () => random);

            return new System.Net.Http.HttpClient(handler).GetAsync(Url);
        }

        private Task RecordDelay(TimeSpan delay, CancellationToken cancellationToken)
        {
            _delays.Add(delay);
            return Task.CompletedTask;
        }
    }
}
