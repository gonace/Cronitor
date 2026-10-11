using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_PingTests : BaseTest
    {
        private const string CompleteUri = "https://cronitor.link/p/" + ApiKey + "/" + MonitorKey + "/complete";

        private readonly FakeHttpMessageHandler _handler;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_PingTests()
        {
            _handler = new FakeHttpMessageHandler();
            _telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
        }

        [Fact]
        public void ShouldExecutePingMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            _telemetriesClient.Ping(command);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        }

        [Fact]
        public async Task ShouldExecutePingAsyncMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            await _telemetriesClient.PingAsync(command);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        }
    }
}
