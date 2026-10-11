using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_FailTests : BaseTest
    {
        private const string Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        private const string FailUri = "https://cronitor.link/p/" + ApiKey + "/" + MonitorKey + "/fail";

        private readonly FakeHttpMessageHandler _handler;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_FailTests()
        {
            _handler = new FakeHttpMessageHandler();
            _telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
        }

        [Fact]
        public void ShouldExecuteFailMethod()
        {
            _telemetriesClient.Fail(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethod()
        {
            await _telemetriesClient.FailAsync(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public void ShouldExecuteFailMethodWithMessage()
        {
            _telemetriesClient.Fail(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Message, query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethodWithMessage()
        {
            await _telemetriesClient.FailAsync(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Message, query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteFailMethodWithEnvironment()
        {
            _telemetriesClient.Fail(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethodWithEnvironment()
        {
            await _telemetriesClient.FailAsync(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteFailMethodWithMessageAndEnvironment()
        {
            _telemetriesClient.Fail(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Message, query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethodWithMessageAndEnvironment()
        {
            await _telemetriesClient.FailAsync(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(FailUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Message, query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }
    }
}
