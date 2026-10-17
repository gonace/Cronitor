using Cronitor.Constants;
using Cronitor.Tests.Helpers;
using System;
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
    public void Should_send_run_from_Start()
    {
        var handler = new FakeHttpMessageHandler();

        TelemetryRun.Start(CreateClient(handler), MonitorKey);

        var request = Assert.Single(handler.Requests);
        Assert.Equal("run", Endpoint(request));
        Assert.False(string.IsNullOrWhiteSpace(Series(request)));
    }

    [Fact]
    public async Task Should_send_run_from_StartAsync()
    {
        var handler = new FakeHttpMessageHandler();

        await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey);

        var request = Assert.Single(handler.Requests);
        Assert.Equal("run", Endpoint(request));
        Assert.False(string.IsNullOrWhiteSpace(Series(request)));
    }

    [Fact]
    public void Should_send_complete_in_the_same_series_from_Complete()
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
    public async Task Should_send_complete_in_the_same_series_from_CompleteAsync()
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
    public async Task Should_send_fail_with_a_message_from_FailAsync()
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
    public void Should_send_fail_from_Dispose_when_not_completed()
    {
        var handler = new FakeHttpMessageHandler();

        using (TelemetryRun.Start(CreateClient(handler), MonitorKey))
        {
        }

        Assert.Equal(new[] { "run", "fail" }, handler.Requests.Select(Endpoint));
        Assert.Equal(Series(handler.Requests[0]), Series(handler.Requests[1]));
    }

    [Fact]
    public void Should_send_complete_only_once()
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
    public async Task Should_send_tick_with_the_message_in_the_same_series_from_AbandonAsync()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey))
        {
            await run.AbandonAsync("The service went offline");
        }

        Assert.Equal(new[] { "run", "tick" }, handler.Requests.Select(Endpoint));
        Assert.Equal(Series(handler.Requests[0]), Series(handler.Requests[1]));
        Assert.Equal("The service went offline", QueryString.Parse(handler.Requests[1].RequestUri)["message"]);
    }

    [Fact]
    public void Should_send_nothing_more_after_a_run_is_abandoned()
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = TelemetryRun.Start(CreateClient(handler), MonitorKey))
        {
            run.Abandon("The service went offline");

            Assert.False(run.Complete());
        }

        Assert.Equal(new[] { "run", "tick" }, handler.Requests.Select(Endpoint));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void Should_throw_and_keep_the_run_open_when_a_run_is_abandoned_without_a_message(string message)
    {
        var handler = new FakeHttpMessageHandler();

        using (var run = TelemetryRun.Start(CreateClient(handler), MonitorKey))
        {
            Assert.Throws<ArgumentNullException>(() => run.Abandon(message));

            Assert.True(run.Complete());
        }

        Assert.Equal(new[] { "run", "complete" }, handler.Requests.Select(Endpoint));
    }

    [Fact]
    public void Should_send_the_environment_with_every_ping()
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
    public void Should_use_a_new_series_for_every_run()
    {
        var handler = new FakeHttpMessageHandler();
        var client = CreateClient(handler);

        TelemetryRun.Start(client, MonitorKey).Complete();
        TelemetryRun.Start(client, MonitorKey).Complete();

        Assert.NotEqual(Series(handler.Requests[0]), Series(handler.Requests[2]));
    }

    [Fact]
    public void Should_return_true_from_Complete_when_the_ping_is_accepted()
    {
        var handler = new FakeHttpMessageHandler();
        var run = TelemetryRun.Start(CreateClient(handler), MonitorKey);

        Assert.True(run.Complete());
    }

    [Fact]
    public void Should_return_false_from_Complete_when_already_finished()
    {
        var handler = new FakeHttpMessageHandler();
        var run = TelemetryRun.Start(CreateClient(handler), MonitorKey);
        run.Fail();

        Assert.False(run.Complete());
    }

    [Fact]
    public async Task Should_not_throw_from_StartAsync_when_run_cannot_be_sent()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"detail\":\"Server error\"}");

        using (var run = await TelemetryRun.StartAsync(CreateClient(handler), MonitorKey))
        {
            Assert.False(await run.CompleteAsync());
        }

        Assert.Equal(new[] { "run", "complete" }, handler.Requests.Select(Endpoint));
    }

    [Fact]
    public void Should_not_throw_from_Dispose_when_fail_cannot_be_sent()
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
