using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

[Collection(StaticCronitorCollection.Name)]
public class CronitorClientTests : BaseTest
{
    private readonly FakeHttpMessageHandler _handler;

    public CronitorClientTests()
    {
        _handler = new FakeHttpMessageHandler();
    }

    [Fact]
    public void Should_configure_CronitorClient()
    {
        CronitorClient.Configure(ApiKey);

        Assert.NotNull(CronitorClient.Issues);
        Assert.NotNull(CronitorClient.Monitors);
        Assert.NotNull(CronitorClient.Notifications);
        Assert.NotNull(CronitorClient.Telemetries);
    }

    [Fact]
    public void Should_execute_Run()
    {
        ConfigureWithFakeHandler();

        CronitorClient.Run(MonitorKey);

        AssertSent("run");
    }

    [Fact]
    public async Task Should_execute_RunAsync()
    {
        ConfigureWithFakeHandler();

        await CronitorClient.RunAsync(MonitorKey);

        AssertSent("run");
    }

    [Fact]
    public void Should_execute_Complete()
    {
        ConfigureWithFakeHandler();

        CronitorClient.Complete(MonitorKey);

        AssertSent("complete");
    }

    [Fact]
    public async Task Should_execute_CompleteAsync()
    {
        ConfigureWithFakeHandler();

        await CronitorClient.CompleteAsync(MonitorKey);

        AssertSent("complete");
    }

    [Fact]
    public void Should_execute_Fail()
    {
        ConfigureWithFakeHandler();

        CronitorClient.Fail(MonitorKey);

        AssertSent("fail");
    }

    [Fact]
    public async Task Should_execute_FailAsync()
    {
        ConfigureWithFakeHandler();

        await CronitorClient.FailAsync(MonitorKey);

        AssertSent("fail");
    }

    [Fact]
    public void Should_execute_Tick()
    {
        ConfigureWithFakeHandler();

        CronitorClient.Tick(MonitorKey);

        AssertSent("tick");
    }

    [Fact]
    public async Task Should_execute_TickAsync()
    {
        ConfigureWithFakeHandler();

        await CronitorClient.TickAsync(MonitorKey);

        AssertSent("tick");
    }

    [Fact]
    public void Should_execute_Ping()
    {
        ConfigureWithFakeHandler();
        var command = new CompleteCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        CronitorClient.Ping(command);

        AssertSent("complete");
    }

    [Fact]
    public async Task Should_execute_PingAsync()
    {
        ConfigureWithFakeHandler();
        var command = new CompleteCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        await CronitorClient.PingAsync(command);

        AssertSent("complete");
    }

    [Fact]
    public void Should_execute_Start()
    {
        ConfigureWithFakeHandler();

        CronitorClient.Start(MonitorKey);

        AssertSent("run");
    }

    [Fact]
    public async Task Should_execute_StartAsync()
    {
        ConfigureWithFakeHandler();

        await CronitorClient.StartAsync(MonitorKey);

        AssertSent("run");
    }

    private void ConfigureWithFakeHandler()
    {
        CronitorClient.Configure(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, _handler));
    }

    private void AssertSent(string endpoint)
    {
        var request = Assert.Single(_handler.Requests);
        Assert.Equal($"https://cronitor.link/p/{ApiKey}/{MonitorKey}/{endpoint}", request.RequestUri.GetLeftPart(UriPartial.Path));
    }
}
