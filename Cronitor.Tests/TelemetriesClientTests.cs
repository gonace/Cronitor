using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClientTests : BaseTest
    {
        private readonly FakeHttpMessageHandler _handler;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClientTests()
        {
            _handler = new FakeHttpMessageHandler();
            _telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
        }

        [Fact]
        public void ShouldConstructWithApiKey()
        {
            var client = new TelemetriesClient(ApiKey);

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldDisposeTelemetriesClient()
        {
            using (var client = new TelemetriesClient(ApiKey))
            {
                Assert.NotNull(client);
            }
            // Test passes if no exception is thrown during disposal
        }

        [Fact]
        public void ShouldExecuteWhenUtilizingUsingBlock()
        {
            _telemetriesClient.Run(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal($"https://cronitor.link/p/{ApiKey}/{MonitorKey}/run", request.RequestUri.GetLeftPart(UriPartial.Path));
        }

        [Fact]
        public void ShouldReturnTrueWhenThePingIsAccepted()
        {
            var accepted = _telemetriesClient.Run(MonitorKey);

            Assert.True(accepted);
        }

        [Fact]
        public async Task ShouldReturnFalseWhenCronitorRespondsWithAnError()
        {
            var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"detail\":\"Server error\"}");
            var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

            var accepted = await telemetriesClient.RunAsync(MonitorKey);

            Assert.False(accepted);
        }

        [Fact]
        public async Task ShouldReturnFalseWhenCronitorRespondsWithoutJson()
        {
            var handler = new FakeHttpMessageHandler(HttpStatusCode.BadGateway, "<html>Bad Gateway</html>");
            var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

            var accepted = await telemetriesClient.CompleteAsync(MonitorKey);

            Assert.False(accepted);
        }

        [Fact]
        public void ShouldReturnFalseWhenCronitorCannotBeReached()
        {
            var network = new ScriptedHttpMessageHandler(ScriptedHttpMessageHandler.NetworkError());
            var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, network));

            var accepted = telemetriesClient.Fail(MonitorKey);

            Assert.False(accepted);
        }

        [Fact]
        public void ShouldStillThrowForAMissingMonitorKey()
        {
            Assert.Throws<ArgumentNullException>(() => _telemetriesClient.Run(" "));
        }

        [Fact]
        public void ShouldStillThrowForACommandWithoutMonitorKey()
        {
            var command = new RunCommand().WithApiKey(ApiKey);

            Assert.Throws<ArgumentNullException>(() => _telemetriesClient.Ping(command));
        }

        [Fact]
        public void ShouldRetryTelemetryPings()
        {
            var network = new ScriptedHttpMessageHandler(
                ScriptedHttpMessageHandler.Status(HttpStatusCode.ServiceUnavailable),
                ScriptedHttpMessageHandler.Status(HttpStatusCode.OK));
            var telemetriesClient = new TelemetriesClient(ApiKey, HttpClient.ForTelemetry(ApiKey, network));

            telemetriesClient.Run(MonitorKey);

            Assert.Equal(2, network.Attempts);
        }

        [Fact]
        public void ShouldExecuteStartMethod()
        {
            ITelemetriesClient telemetriesClient = _telemetriesClient;

            using (var run = telemetriesClient.Start(MonitorKey, Environment))
            {
                run.Complete();
            }

            Assert.Equal(new[] { "run", "complete" }, _handler.Requests.Select(r => r.RequestUri.Segments.Last()));
            Assert.All(_handler.Requests, r => Assert.Equal(Environment, QueryString.Parse(r.RequestUri)["env"]));
        }

        [Fact]
        public async Task ShouldExecuteStartAsyncMethod()
        {
            ITelemetriesClient telemetriesClient = _telemetriesClient;

            using (await telemetriesClient.StartAsync(MonitorKey))
            {
            }

            Assert.Equal(new[] { "run", "fail" }, _handler.Requests.Select(r => r.RequestUri.Segments.Last()));
        }
    }
}
