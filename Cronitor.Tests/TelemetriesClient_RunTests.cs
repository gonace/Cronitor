using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

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
    public void Should_execute_Run()
    {
        _telemetriesClient.Run(MonitorKey);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Empty(QueryString.Parse(request.RequestUri));
    }

    [Fact]
    public async Task Should_execute_RunAsync()
    {
        await _telemetriesClient.RunAsync(MonitorKey);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Empty(QueryString.Parse(request.RequestUri));
    }

    [Fact]
    public void Should_execute_Run_with_a_message()
    {
        _telemetriesClient.Run(MonitorKey, Message);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Single(query);
    }

    [Fact]
    public async Task Should_execute_RunAsync_with_a_message()
    {
        await _telemetriesClient.RunAsync(MonitorKey, Message);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Single(query);
    }

    [Fact]
    public void Should_execute_Run_with_an_environment()
    {
        _telemetriesClient.Run(MonitorKey, environment: Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Environment, query["env"]);
        Assert.Single(query);
    }

    [Fact]
    public async Task Should_execute_RunAsync_with_an_environment()
    {
        await _telemetriesClient.RunAsync(MonitorKey, environment: Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Environment, query["env"]);
        Assert.Single(query);
    }

    [Fact]
    public void Should_execute_Run_with_a_message_and_an_environment()
    {
        _telemetriesClient.Run(MonitorKey, Message, Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Equal(Environment, query["env"]);
        Assert.Equal(2, query.Count);
    }

    [Fact]
    public async Task Should_execute_RunAsync_with_a_message_and_an_environment()
    {
        await _telemetriesClient.RunAsync(MonitorKey, Message, Environment);

        var request = Assert.Single(_handler.Requests);
        var query = QueryString.Parse(request.RequestUri);
        Assert.Equal(RunUri, request.RequestUri.GetLeftPart(UriPartial.Path));
        Assert.Equal(Message, query["message"]);
        Assert.Equal(Environment, query["env"]);
        Assert.Equal(2, query.Count);
    }

    [Theory]
    [InlineData("a & b")]
    [InlineData("50% done #3")]
    [InlineData("it's ok")]
    [InlineData("x=1+2")]
    public void Should_send_the_message_unchanged(string message)
    {
        _telemetriesClient.Run(MonitorKey, message);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal(message, QueryString.Parse(request.RequestUri)["message"]);
    }

    [Fact]
    public void Should_send_the_environment_unchanged()
    {
        const string environment = "staging & qa";

        _telemetriesClient.Run(MonitorKey, environment: environment);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal(environment, QueryString.Parse(request.RequestUri)["env"]);
    }

    [Fact]
    public void Should_url_encode_query_values()
    {
        _telemetriesClient.Run(MonitorKey, "a & b");

        var request = Assert.Single(_handler.Requests);
        Assert.Equal($"{RunUri}?message=a%20%26%20b", request.RequestUri.AbsoluteUri);
    }
}
