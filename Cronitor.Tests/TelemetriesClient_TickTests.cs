using Cronitor.Clients;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_TickTests : BaseTest
    {
        private const string Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        private const string TickUri = "https://cronitor.link/p/" + ApiKey + "/" + MonitorKey + "/tick";

        private readonly FakeHttpMessageHandler _handler;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_TickTests()
        {
            _handler = new FakeHttpMessageHandler();
            _telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
        }

        [Fact]
        public void ShouldExecuteTickMethod()
        {
            _telemetriesClient.Tick(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethod()
        {
            await _telemetriesClient.TickAsync(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public void ShouldExecuteTickMethodWithMessage()
        {
            _telemetriesClient.Tick(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethodWithMessage()
        {
            await _telemetriesClient.TickAsync(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteTickMethodWithEnvironment()
        {
            _telemetriesClient.Tick(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethodWithEnvironment()
        {
            await _telemetriesClient.TickAsync(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteTickMethodWithMessageAndEnvironment()
        {
            _telemetriesClient.Tick(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethodWithMessageAndEnvironment()
        {
            await _telemetriesClient.TickAsync(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(TickUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }
    }
}
