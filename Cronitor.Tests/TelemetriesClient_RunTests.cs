using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_RunTests : BaseTest
    {
        private const string Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        private const string RunUri = "https://cronitor.link/p/" + ApiKey + "/" + MonitorKey + "/run";

        private readonly FakeHttpMessageHandler _handler;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_RunTests()
        {
            _handler = new FakeHttpMessageHandler();
            _telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
        }

        [Fact]
        public void ShouldExecuteRunMethod()
        {
            _telemetriesClient.Run(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethod()
        {
            await _telemetriesClient.RunAsync(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public void ShouldExecuteRunMethodWithMessage()
        {
            _telemetriesClient.Run(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithMessage()
        {
            await _telemetriesClient.RunAsync(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteRunMethodWithEnvironment()
        {
            _telemetriesClient.Run(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithEnvironment()
        {
            await _telemetriesClient.RunAsync(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteRunMethodWithMessageAndEnvironment()
        {
            _telemetriesClient.Run(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithMessageAndEnvironment()
        {
            await _telemetriesClient.RunAsync(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }
    }
}
