using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

[Collection(StaticCronitorCollection.Name)]
public class CronitorTests : BaseTest
{
    private readonly FakeHttpMessageHandler _handler;

    public CronitorTests()
    {
        _handler = new FakeHttpMessageHandler();
    }

    [Fact]
    public void Should_configure_Cronitor()
    {
        Cronitor.Configure(ApiKey);

        Assert.NotNull(Cronitor.Issues);
        Assert.NotNull(Cronitor.Monitors);
        Assert.NotNull(Cronitor.Notifications);
        Assert.NotNull(Cronitor.Telemetries);
    }

    [Fact]
    public void Should_execute_Run()
    {
        ConfigureWithFakeHandler();

        Cronitor.Run(MonitorKey);

        AssertSent("run");
    }

    [Fact]
    public async Task Should_execute_RunAsync()
    {
        ConfigureWithFakeHandler();

        await Cronitor.RunAsync(MonitorKey);

        AssertSent("run");
    }

    [Fact]
    public void Should_execute_Complete()
    {
        ConfigureWithFakeHandler();

        Cronitor.Complete(MonitorKey);

        AssertSent("complete");
    }

    [Fact]
    public async Task Should_execute_CompleteAsync()
    {
        ConfigureWithFakeHandler();

        await Cronitor.CompleteAsync(MonitorKey);

        AssertSent("complete");
    }

    [Fact]
    public void Should_execute_Fail()
    {
        ConfigureWithFakeHandler();

        Cronitor.Fail(MonitorKey);

        AssertSent("fail");
    }

    [Fact]
    public async Task Should_execute_FailAsync()
    {
        ConfigureWithFakeHandler();

        await Cronitor.FailAsync(MonitorKey);

        AssertSent("fail");
    }

    [Fact]
    public void Should_execute_Tick()
    {
        ConfigureWithFakeHandler();

        Cronitor.Tick(MonitorKey);

        AssertSent("tick");
    }

    [Fact]
    public async Task Should_execute_TickAsync()
    {
        ConfigureWithFakeHandler();

        await Cronitor.TickAsync(MonitorKey);

        AssertSent("tick");
    }

    [Fact]
    public void Should_execute_Ping()
    {
        ConfigureWithFakeHandler();
        var command = new CompleteCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        Cronitor.Ping(command);

        AssertSent("complete");
    }

    [Fact]
    public async Task Should_execute_PingAsync()
    {
        ConfigureWithFakeHandler();
        var command = new CompleteCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        await Cronitor.PingAsync(command);

        AssertSent("complete");
    }

    [Fact]
    public void Should_execute_Start()
    {
        ConfigureWithFakeHandler();

        Cronitor.Start(MonitorKey);

        AssertSent("run");
    }

    [Fact]
    public async Task Should_execute_StartAsync()
    {
        ConfigureWithFakeHandler();

        await Cronitor.StartAsync(MonitorKey);

        AssertSent("run");
    }

    private void ConfigureWithFakeHandler()
    {
        Cronitor.Configure(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
    }

    private void AssertSent(string endpoint)
    {
        var request = Assert.Single(_handler.Requests);
        Assert.Equal($"https://cronitor.link/p/{ApiKey}/{MonitorKey}/{endpoint}", request.RequestUri.GetLeftPart(UriPartial.Path));
    }
}
