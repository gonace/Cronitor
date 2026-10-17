using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Coravel;

/// <summary>
/// Reports the runs of the jobs scheduled with <c>ScheduleWithCronitor</c> to Cronitor: run when the job starts, then
/// complete when it succeeds or fail when it throws. Reporting never affects the job: the start is reported in the
/// background without the job waiting for it, the end once the job is done, and every failure is logged instead of thrown.
/// </summary>
internal sealed class CronitorReporter
{
    private const string CancelledMessage = "The job was cancelled";
    private const string OfflineMessage = "The service went offline";

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorReporter> _logger;
    private readonly CancellationToken _applicationStopping;
    private readonly ConcurrentDictionary<Task, bool> _pendingReports = new ConcurrentDictionary<Task, bool>();

    public CronitorReporter(ITelemetriesClient telemetries, ILogger<CronitorReporter> logger = null, CancellationToken applicationStopping = default)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorReporter>.Instance;
        _applicationStopping = applicationStopping;
    }

    /// <summary>
    /// Runs the job and reports it to the monitor with the given key. The returned task is the job's own, so Coravel
    /// still sees the job's exception. The cancellation token is the one Coravel gave the job.
    /// </summary>
    internal Task RunAsync(MonitorKey monitorKey, Func<Task> job, CancellationToken cancellationToken)
    {
        var start = ReportStartAsync(monitorKey);
        var execution = ExecuteAsync(job);

        Track(ReportEndAsync(start, execution, monitorKey, cancellationToken));

        return execution;
    }

    /// <summary>
    /// Waits for the reports still being sent, at most for the given time, so they are not lost when the application
    /// stops. <see cref="CronitorShutdownService"/> calls it once the scheduler has stopped.
    /// </summary>
    internal async Task WaitForPendingReportsAsync(TimeSpan limit, CancellationToken cancellationToken)
    {
        var pending = _pendingReports.Keys.ToArray();
        if (pending.Length == 0)
        {
            return;
        }

        var sent = Task.WhenAll(pending);
        if (await Task.WhenAny(sent, Task.Delay(limit, cancellationToken)).ConfigureAwait(false) != sent)
        {
            _logger.ReportsPendingAtShutdown(_pendingReports.Count);
        }
    }

    private void Track(Task report)
    {
        _pendingReports.TryAdd(report, true);
        report.ContinueWith(sent => _pendingReports.TryRemove(sent, out _), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    /// <summary>
    /// Runs the job, turning an exception it throws before returning a task into a faulted task, so the end of the run
    /// is still reported.
    /// </summary>
    private static Task ExecuteAsync(Func<Task> job)
    {
        try
        {
            return job();
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    /// <summary>
    /// Sends the run event. The returned task completes with the run, or with null if it could not be reported.
    /// </summary>
    private async Task<ITelemetryRun> ReportStartAsync(MonitorKey monitorKey)
    {
        try
        {
            return await _telemetries.StartAsync(monitorKey).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, monitorKey);
            return null;
        }
    }

    /// <summary>
    /// Sends abandon, complete or fail once the job has ended and the run event has been sent, so Cronitor always receives them in order.
    /// </summary>
    private async Task ReportEndAsync(Task<ITelemetryRun> start, Task execution, MonitorKey monitorKey, CancellationToken cancellationToken)
    {
        try
        {
            var (abandonment, failure) = await OutcomeOfAsync(execution, cancellationToken).ConfigureAwait(false);
            using (var run = await start.ConfigureAwait(false))
            {
                if (run == null)
                {
                    return;
                }

                if (abandonment != null)
                {
                    await run.AbandonAsync(abandonment).ConfigureAwait(false);
                }
                else if (failure == null)
                {
                    await run.CompleteAsync().ConfigureAwait(false);
                }
                else
                {
                    await run.FailAsync(failure).ConfigureAwait(false);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, monitorKey);
        }
    }

    /// <summary>
    /// Why the job was stopped before it could end, or why it did not succeed; both null if it succeeded. Coravel
    /// cancels the jobs still running when the application stops, so a job Coravel cancelled while the application is
    /// stopping is abandoned instead of failed.
    /// </summary>
    private async Task<(string Abandonment, string Failure)> OutcomeOfAsync(Task execution, CancellationToken cancellationToken)
    {
        try
        {
            await execution.ConfigureAwait(false);
            return (null, null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested && _applicationStopping.IsCancellationRequested)
        {
            return (OfflineMessage, null);
        }
        catch (OperationCanceledException)
        {
            return (null, CancelledMessage);
        }
        catch (Exception exception)
        {
            return (null, exception.GetBaseException().Message);
        }
    }
}
