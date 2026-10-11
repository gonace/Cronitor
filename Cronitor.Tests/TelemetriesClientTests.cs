using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

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
    public void Should_construct_with_an_api_key()
    {
        var client = new TelemetriesClient(ApiKey);

        Assert.NotNull(client);
    }

    [Fact]
    public void Should_dispose_the_TelemetriesClient()
    {
        using (var client = new TelemetriesClient(ApiKey))
        {
            Assert.NotNull(client);
        }
        // Test passes if no exception is thrown during disposal
    }

    [Fact]
    public void Should_execute_in_a_using_block()
    {
        _telemetriesClient.Run(MonitorKey);

        var request = Assert.Single(_handler.Requests);
        Assert.Equal($"https://cronitor.link/p/{ApiKey}/{MonitorKey}/run", request.RequestUri.GetLeftPart(UriPartial.Path));
    }

    [Fact]
    public void Should_return_true_when_the_ping_is_accepted()
    {
        var accepted = _telemetriesClient.Run(MonitorKey);

        Assert.True(accepted);
    }

    [Fact]
    public async Task Should_return_false_when_Cronitor_responds_with_an_error()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"detail\":\"Server error\"}");
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

        var accepted = await telemetriesClient.RunAsync(MonitorKey);

        Assert.False(accepted);
    }

    [Fact]
    public async Task Should_return_false_when_Cronitor_responds_without_json()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.BadGateway, "<html>Bad Gateway</html>");
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

        var accepted = await telemetriesClient.CompleteAsync(MonitorKey);

        Assert.False(accepted);
    }

    [Fact]
    public void Should_return_false_when_Cronitor_cannot_be_reached()
    {
        var network = new ScriptedHttpMessageHandler(ScriptedHttpMessageHandler.NetworkError());
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, network));

        var accepted = telemetriesClient.Fail(MonitorKey);

        Assert.False(accepted);
    }

    [Fact]
    public void Should_log_an_error_when_the_ping_cannot_be_delivered()
    {
        var loggerFactory = new FakeLoggerFactory();
        var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"detail\":\"Server error\"}");
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler), loggerFactory);

        telemetriesClient.Run(MonitorKey);

        var entry = Assert.Single(loggerFactory.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal(typeof(TelemetriesClient).FullName, entry.Category);
        Assert.Equal($"Could not deliver run event for monitor {MonitorKey} to Cronitor", entry.Message);
        Assert.IsType<ApiException>(entry.Exception);
    }

    [Fact]
    public void Should_not_log_the_api_key()
    {
        var loggerFactory = new FakeLoggerFactory();
        var network = new ScriptedHttpMessageHandler(ScriptedHttpMessageHandler.NetworkError());
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, network), loggerFactory);

        telemetriesClient.Run(MonitorKey);

        var entry = Assert.Single(loggerFactory.Entries);
        Assert.DoesNotContain(ApiKey, entry.Message);
        Assert.DoesNotContain(entry.Values, value => value.Value?.ToString().Contains(ApiKey) == true);
    }

    [Fact]
    public void Should_not_log_when_the_ping_is_accepted()
    {
        var loggerFactory = new FakeLoggerFactory();
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler), loggerFactory);

        telemetriesClient.Run(MonitorKey);

        Assert.Empty(loggerFactory.Entries);
    }

    [Fact]
    public void Should_require_a_logger_factory()
    {
        Assert.Throws<ArgumentNullException>(() => new TelemetriesClient(ApiKey, (ILoggerFactory)null));
    }

    [Fact]
    public void Should_return_false_when_every_attempt_times_out()
    {
        var network = new ScriptedHttpMessageHandler(ScriptedHttpMessageHandler.Hang());
        var retrying = new RetryingHttpMessageHandler(network, (_, _) => Task.CompletedTask, () => 0.5, TimeSpan.FromMilliseconds(50));
        var telemetriesClient = new TelemetriesClient(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, retrying));

        var accepted = telemetriesClient.Run(MonitorKey);

        Assert.False(accepted);
        Assert.Equal(4, network.Attempts);
    }

    [Fact]
    public void Should_still_throw_for_a_missing_monitor_key()
    {
        Assert.Throws<ArgumentNullException>(() => _telemetriesClient.Run(" "));
    }

    [Fact]
    public void Should_still_throw_for_a_command_without_a_monitor_key()
    {
        var command = new RunCommand().WithApiKey(ApiKey);

        Assert.Throws<ArgumentNullException>(() => _telemetriesClient.Ping(command));
    }

    [Fact]
    public void Should_retry_telemetry_pings()
    {
        var network = new ScriptedHttpMessageHandler(
            ScriptedHttpMessageHandler.Status(HttpStatusCode.ServiceUnavailable),
            ScriptedHttpMessageHandler.Status(HttpStatusCode.OK));
        var telemetriesClient = new TelemetriesClient(ApiKey, HttpClient.ForTelemetry(ApiKey, network));

        telemetriesClient.Run(MonitorKey);

        Assert.Equal(2, network.Attempts);
    }

    [Fact]
    public void Should_execute_Start()
    {
        ITelemetriesClient telemetriesClient = _telemetriesClient;

        using (var run = telemetriesClient.Start(MonitorKey, Environment))
        {
            run.Complete();
        }

        Assert.Equal(new[] { "run", "complete" }, _handler.Requests.Select(r => r.RequestUri.Segments.Last()));
        Assert.All(_handler.Requests, r => Assert.Equal(Environment, QueryString.Parse(r.RequestUri)["env"]));
    }

    [Fact]
    public async Task Should_execute_StartAsync()
    {
        ITelemetriesClient telemetriesClient = _telemetriesClient;

        using (await telemetriesClient.StartAsync(MonitorKey))
        {
        }

        Assert.Equal(new[] { "run", "fail" }, _handler.Requests.Select(r => r.RequestUri.Segments.Last()));
    }
}
