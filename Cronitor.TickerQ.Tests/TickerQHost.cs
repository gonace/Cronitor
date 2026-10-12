using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.DependencyInjection;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Interfaces.Managers;
using Xunit;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// One TickerQ host, set up the way an application would with <c>AddCronitorTickerQ()</c>, shared by every test in
/// <see cref="TickerQHostCollection"/>. TickerQ 10.4 does not reliably fire cron tickers in a host started after
/// another host in the same process has stopped, so the tests share a single host, like an application does.
/// </summary>
public sealed class TickerQHost : IAsyncLifetime
{
    private readonly ForwardingTelemetriesClient _telemetries = new ForwardingTelemetriesClient();
    private IHost _host;

    public async ValueTask InitializeAsync()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<ITelemetriesClient>(_telemetries);
        builder.Services.AddTickerQ();
        builder.Services.AddCronitorTickerQ();

        _host = builder.Build();
        _host.UseTickerQ();
        await _host.StartAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync(CancellationToken.None);
        _host.Dispose();
    }

    /// <summary>
    /// Sends what the host reports to Cronitor to the given client from now on.
    /// </summary>
    public FakeTelemetriesClient ReportTo(FakeTelemetriesClient telemetries)
    {
        _telemetries.Target = telemetries;
        return telemetries;
    }

    /// <summary>
    /// Schedules the function to run once a minute, at a second two seconds from now, so the test sees exactly one
    /// occurrence. Disposing the result removes the cron ticker again, so it doesn't fire during later tests.
    /// </summary>
    public async Task<IAsyncDisposable> AddCronTicker(string function, int retries = 0)
    {
        var second = DateTime.UtcNow.AddSeconds(2).Second;
        var cronTickers = _host.Services.GetRequiredService<ICronTickerManager<CronTickerEntity>>();
        var cronTicker = new CronTickerEntity { Function = function, Expression = $"{second} * * * * *", Retries = retries, RetryIntervals = [1] };

        var result = await cronTickers.AddAsync(cronTicker, TestContext.Current.CancellationToken);

        Assert.True(result.IsSucceeded, $"TickerQ refused the cron ticker for {function}.");
        return new CronTickerRemoval(cronTickers, cronTicker.Id);
    }

    private sealed class CronTickerRemoval(ICronTickerManager<CronTickerEntity> cronTickers, Guid id) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync() => await cronTickers.DeleteAsync(id, CancellationToken.None);
    }
}

/// <summary>
/// The tests that share the <see cref="TickerQHost"/>. They run one at a time, since they share its telemetry client.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public class TickerQHostCollection : ICollectionFixture<TickerQHost>
{
    public const string Name = "TickerQ host";
}
