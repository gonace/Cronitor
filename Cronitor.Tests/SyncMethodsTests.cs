using Cronitor.Constants;
using Cronitor.Tests.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

/// <summary>
/// Calls the sync methods where a synchronization context waits for the blocked thread, as in WinForms, WPF or
/// classic ASP.NET, with a network that answers later, so the awaited work has to continue somewhere.
/// </summary>
public class SyncMethodsTests : BaseTest
{
    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(5);

    [Fact]
    public void Should_return_from_Run()
    {
        var client = new TelemetriesClient(ApiKey, Internals.HttpClient.ForTelemetry(ApiKey, new ScriptedHttpMessageHandler(RespondLater("{}"))));
        var delivered = false;

        var finished = SingleThreadedContext.Run(() => delivered = client.Run(MonitorKey), Limit);

        Assert.True(finished);
        Assert.True(delivered);
    }

    [Fact]
    public void Should_report_fail_when_a_run_is_disposed_without_being_completed()
    {
        var handler = new ScriptedHttpMessageHandler(RespondLater("{}"));
        var client = new TelemetriesClient(ApiKey, Internals.HttpClient.ForTelemetry(ApiKey, handler));

        var finished = SingleThreadedContext.Run(() => client.Start(MonitorKey).Dispose(), Limit);

        Assert.True(finished);
        Assert.Equal(2, handler.Attempts);
    }

    [Fact]
    public void Should_return_from_Get_on_the_monitors_client()
    {
        var client = new MonitorsClient(new Internals.HttpClient(Urls.DefaultApiUrl, ApiKey, new ScriptedHttpMessageHandler(RespondLater("{\"key\":\"nightly-backup\"}"))));
        Models.Monitor monitor = null;

        var finished = SingleThreadedContext.Run(() => monitor = client.Get("nightly-backup"), Limit);

        Assert.True(finished);
        Assert.Equal("nightly-backup", monitor.Key.ToString());
    }

    private static Func<CancellationToken, Task<HttpResponseMessage>> RespondLater(string body) =>
        async _ =>
        {
            await Task.Delay(50).ConfigureAwait(false);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
        };
}
