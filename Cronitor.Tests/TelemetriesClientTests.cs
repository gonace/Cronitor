using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
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
        public void ShouldConstructWithNoParameters()
        {
            var client = new TelemetriesClient();

            Assert.NotNull(client);
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
    }
}
