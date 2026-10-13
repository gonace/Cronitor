using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using Xunit;

#pragma warning disable CS0618 // Cronitor is obsolete; these tests check that it still forwards to CronitorClient.

namespace Cronitor.Tests;

/// <summary>
/// The former static class <see cref="Cronitor"/> keeps working for existing code by forwarding to <see cref="CronitorClient"/>.
/// </summary>
[Collection(StaticCronitorCollection.Name)]
public class CronitorTests : BaseTest
{
    [Fact]
    public void Should_configure_CronitorClient_when_Cronitor_is_configured()
    {
        Cronitor.Configure(ApiKey);

        Assert.Same(CronitorClient.Issues, Cronitor.Issues);
        Assert.Same(CronitorClient.Monitors, Cronitor.Monitors);
        Assert.Same(CronitorClient.Notifications, Cronitor.Notifications);
        Assert.Same(CronitorClient.Telemetries, Cronitor.Telemetries);
    }

    [Fact]
    public void Should_send_telemetry_through_CronitorClient()
    {
        var handler = new FakeHttpMessageHandler();
        CronitorClient.Configure(ApiKey, new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

        Cronitor.Run(MonitorKey);

        var request = Assert.Single(handler.Requests);
        Assert.Equal($"https://cronitor.link/p/{ApiKey}/{MonitorKey}/run", request.RequestUri.GetLeftPart(UriPartial.Path));
    }
}
