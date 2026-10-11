using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

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
    public void Should_execute_Complete()
    {
        _telemetriesClient.Complete(MonitorKey);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Empty(QueryString.Parse(request.RequestUri));
    }

    [Fact]
    public async Task Should_execute_CompleteAsync()
    {
        await _telemetriesClient.CompleteAsync(MonitorKey);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Empty(QueryString.Parse(request.RequestUri));
    }

    [Fact]
    public void Should_execute_Complete_with_a_message()
    {
        _telemetriesClient.Complete(MonitorKey, Message);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Single(query);
    }

    [Fact]
    public async Task Should_execute_CompleteAsync_with_a_message()
    {
        await _telemetriesClient.CompleteAsync(MonitorKey, Message);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Single(query);
    }

    [Fact]
    public void Should_execute_Complete_with_an_environment()
    {
        _telemetriesClient.Complete(MonitorKey, environment: Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Environment, query["env"]);
        Assert.Single(query);
    }

    [Fact]
    public async Task Should_execute_CompleteAsync_with_an_environment()
    {
        await _telemetriesClient.CompleteAsync(MonitorKey, environment: Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Environment, query["env"]);
        Assert.Single(query);
    }

    [Fact]
    public void Should_execute_Complete_with_a_message_and_an_environment()
    {
        _telemetriesClient.Complete(MonitorKey, Message, Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Equal(Environment, query["env"]);
        Assert.Equal(2, query.Count);
    }

    [Fact]
    public async Task Should_execute_CompleteAsync_with_a_message_and_an_environment()
    {
        await _telemetriesClient.CompleteAsync(MonitorKey, Message, Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(CompleteUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Equal(Environment, query["env"]);
        Assert.Equal(2, query.Count);
    }
}
