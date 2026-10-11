using Cronitor.Constants;
using Cronitor.Tests.Helpers;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using HttpClient = Cronitor.Internals.HttpClient;

namespace Cronitor.Tests;

public class TelemetryRunTests : BaseTest
{
    private const string Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

    [Fact]
    public void StartShouldSendRun()
    {
        var handler = new FakeHttpMessageHandler();

        TelemetryRun.Start(CreateClient(handler), MonitorKey);

        var request = Assert.Single(handler.Requests);
        Assert.Equal("run", Endpoint(request));
        Assert.False(string.IsNullOrWhiteSpace(Series(request)));
    }

    [Fact]
    public async Task StartAsyncShouldSendRun()
    {
        var handler = new FakeHttpMessageHandler();

        await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey);

        var request = Assert.Single(handler.Requests);
        Assert.Equal("run", Endpoint(request));
        Assert.False(string.IsNullOrWhiteSpace(Series(request)));
    }

    [Fact]
    public void CompleteShouldSendCompleteInTheSameSeries()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = TelemetryRun.Start(CreateClient(handler), MonitorKey))
        {
            run.Complete();
        }

        Assert.Equal(new[] { "run", "complete" }, handler.Requests.Select(Endpoint));
        Assert.Equal(Series(handler.Requests[0]), Series(handler.Requests[1]));
    }

    [Fact]
    public async Task CompleteAsyncShouldSendCompleteInTheSameSeries()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey))
        {
            await run.CompleteAsync();
        }

        Assert.Equal(new[] { "run", "complete" }, handler.Requests.Select(Endpoint));
        Assert.Equal(Series(handler.Requests[0]), Series(handler.Requests[1]));
    }

    [Fact]
    public async Task FailAsyncShouldSendFailWithMessage()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey))
        {
            await run.FailAsync(Message);
        }

        Assert.Equal(new[] { "run", "fail" }, handler.Requests.Select(Endpoint));
        Assert.Equal(Message, QueryString.Parse(handler.Requests[1].RequestUri)["message"]);
    }

    [Fact]
    public void DisposeShouldSendFailWhenNotCompleted()
    {
        var handler = new FakeHttpMessageHandler();

        using (TelemetryRun.Start(CreateClient(handler), MonitorKey))
        {
        }

        Assert.Equal(new[] { "run", "fail" }, handler.Requests.Select(Endpoint));
        Assert.Equal(Series(handler.Requests[0]), Series(handler.Requests[1]));
    }

    [Fact]
    public void CompleteShouldOnlyBeSentOnce()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = TelemetryRun.Start(CreateClient(handler), MonitorKey))
        {
            run.Complete();
            run.Complete();
            run.Fail();
        }

        Assert.Equal(new[] { "run", "complete" }, handler.Requests.Select(Endpoint));
    }

    [Fact]
    public void ShouldSendEnvironmentWithEveryPing()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = TelemetryRun.Start(CreateClient(handler), MonitorKey, Environment))
        {
            run.Complete();
        }

        Assert.All(handler.Requests, request =>
            Assert.Equal(Environment, QueryString.Parse(request.RequestUri)["env"]));
    }

    [Fact]
    public void ShouldUseANewSeriesForEveryRun()
    {
        var handler = new FakeHttpMessageHandler();
        var client = CreateClient(handler);

        TelemetryRun.Start(client, MonitorKey).Complete();
        TelemetryRun.Start(client, MonitorKey).Complete();

        Assert.NotEqual(Series(handler.Requests[0]), Series(handler.Requests[2]));
    }

    [Fact]
    public void CompleteShouldReturnTrueWhenThePingIsAccepted()
    {
        var handler = new FakeHttpMessageHandler();
        var run = TelemetryRun.Start(CreateClient(handler), MonitorKey);

        Assert.True(run.Complete());
    }

    [Fact]
    public void CompleteShouldReturnFalseWhenAlreadyFinished()
    {
        var handler = new FakeHttpMessageHandler();
        var run = TelemetryRun.Start(CreateClient(handler), MonitorKey);
        run.Fail();

        Assert.False(run.Complete());
    }

    [Fact]
    public async Task StartAsyncShouldNotThrowWhenRunCannotBeSent()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"detail\":\"Server error\"}");

        using (var run = await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey))
        {
            Assert.False(await run.CompleteAsync());
        }

        Assert.Equal(new[] { "run", "complete" }, handler.Requests.Select(Endpoint));
    }

    [Fact]
    public void DisposeShouldNotThrowWhenFailCannotBeSent()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"detail\":\"Server error\"}");
        var run = new TelemetryRun(CreateClient(handler), MonitorKey, environment: null);

        run.Dispose();

        Assert.Equal("fail", Endpoint(Assert.Single(handler.Requests)));
    }

    private static TelemetriesClient CreateClient(FakeHttpMessageHandler handler)
    {
        return new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));
    }

    private static string Endpoint(HttpRequestMessage request)
    {
        return request.RequestUri.Segments.Last();
    }

    private static string Series(HttpRequestMessage request)
    {
        return QueryString.Parse(request.RequestUri)["series"];
    }
}
