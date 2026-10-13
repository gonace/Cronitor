using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Coravel;

/// <summary>
/// Waits, when the application stops, for the reports a <see cref="CronitorReporter"/> is still sending, so the outcome
/// of the last jobs reaches Cronitor. Coravel cancels and waits for running jobs when it stops, so this waits after
/// every hosted service has stopped, at most five seconds.
/// </summary>
internal sealed class CronitorShutdownService : IHostedLifecycleService
{
    private readonly CronitorReporter _reporter;
    private readonly TimeSpan _limit;

    public CronitorShutdownService(CronitorReporter reporter)
        : this(reporter, TimeSpan.FromSeconds(5))
    {
    }

    internal CronitorShutdownService(CronitorReporter reporter, TimeSpan limit)
    {
        _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
        _limit = limit;
    }

    public Task StoppedAsync(CancellationToken cancellationToken) => _reporter.WaitForPendingReportsAsync(_limit, cancellationToken);

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
