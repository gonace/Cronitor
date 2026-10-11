using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_CompleteTests : BaseTest
    {
        private const string Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        private const string CompleteUri = "https://cronitor.link/p/" + ApiKey + "/" + MonitorKey + "/complete";

        private readonly FakeHttpMessageHandler _handler;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_CompleteTests()
        {
            _handler = new FakeHttpMessageHandler();
            _telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
        }

        [Fact]
        public void ShouldExecuteCompleteMethod()
        {
            _telemetriesClient.Complete(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethod()
        {
            await _telemetriesClient.CompleteAsync(MonitorKey);

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Empty(QueryString.Parse(request.RequestUri));
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithMessage()
        {
            _telemetriesClient.Complete(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethodWithMessage()
        {
            await _telemetriesClient.CompleteAsync(MonitorKey, Message);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithEnvironment()
        {
            _telemetriesClient.Complete(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethodWithEnvironment()
        {
            await _telemetriesClient.CompleteAsync(MonitorKey, environment: Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal(Environment, query["env"]);
            Assert.Single(query);
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithMessageAndEnvironment()
        {
            _telemetriesClient.Complete(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethodWithMessageAndEnvironment()
        {
            await _telemetriesClient.CompleteAsync(MonitorKey, Message, Environment);

            var request = Assert.Single(_handler.Requests);
            var query = QueryString.Parse(request.RequestUri);
            Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
            Assert.Equal($"'{Message}'", query["message"]);
            Assert.Equal(Environment, query["env"]);
            Assert.Equal(2, query.Count);
        }
    }
}
