using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.TickerQ;

/// <summary>
/// Waits, when the application stops, for the cron jobs a <see cref="CronitorExecutionTaskHandler"/> is still
/// reporting, so the outcome of the last jobs reaches Cronitor. TickerQ cancels running jobs when it stops but does
/// not wait for them, so this waits after every hosted service has stopped, at most five seconds.
/// </summary>
internal sealed class CronitorShutdownService : IHostedLifecycleService
{
    private readonly CronitorExecutionTaskHandler _handler;
    private readonly TimeSpan _limit;

    public CronitorShutdownService(CronitorExecutionTaskHandler handler)
        : this(handler, TimeSpan.FromSeconds(5))
    {
    }

    internal CronitorShutdownService(CronitorExecutionTaskHandler handler, TimeSpan limit)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _limit = limit;
    }

    public Task StoppedAsync(CancellationToken cancellationToken) => _handler.WaitForPendingReportsAsync(_limit, cancellationToken);

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
