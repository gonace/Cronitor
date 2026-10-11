using Cronitor;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hangfire;

/// <summary>
/// Reports every recurring job Hangfire runs to Cronitor: run when the job starts, then complete when it
/// succeeds or fail when it throws. The recurring job's ID is used as the monitor key; other jobs are not reported.
/// Reporting never affects the job: Hangfire runs filters on the job's worker, skips a job when a filter throws
/// before it, and marks a successful job as failed when a filter throws after it. So reports are sent in the
/// background, without the worker waiting for Cronitor, and every failure is logged instead of thrown.
/// </summary>
public class CronitorJobFilter : IServerFilter
{
    private const string StartKey = "Cronitor.Start";
    private const string RecurringJobIdParameter = "RecurringJobId";
    private static readonly TimeSpan ShutdownLimit = TimeSpan.FromSeconds(5);

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorJobFilter> _logger;
    private readonly ConcurrentDictionary<Task, bool> _pendingReports = new ConcurrentDictionary<Task, bool>();

    public CronitorJobFilter(ITelemetriesClient telemetries, ILogger<CronitorJobFilter> logger = null)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorJobFilter>.Instance;

        // Hangfire has no shutdown event for filters. The process exits after the host has stopped Hangfire's
        // workers, so reports still being sent then are the last ones, and are worth a short wait.
        AppDomain.CurrentDomain.ProcessExit += (_, _) => WaitForPendingReports(ShutdownLimit);
    }

    public void OnPerforming(PerformingContext context)
    {
        try
        {
            var monitorKey = context.GetJobParameter<string>(RecurringJobIdParameter);
            if (string.IsNullOrWhiteSpace(monitorKey))
            {
                return;
            }

            context.Items[StartKey] = ReportStart(monitorKey, context.BackgroundJob.Id);
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, context.BackgroundJob.Id);
        }
    }

    public void OnPerformed(PerformedContext context)
    {
        try
        {
            if (context.Items.TryGetValue(StartKey, out var value) && value is Task<ITelemetryRun> start)
            {
                Track(ReportEnd(start, FailureOf(context), context.BackgroundJob.Id));
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.BackgroundJob.Id);
        }
    }

    /// <summary>
    /// Waits for the end reports still being sent, at most for the given time, so they are not lost when the
    /// application stops.
    /// </summary>
    internal void WaitForPendingReports(TimeSpan limit)
    {
        var pending = _pendingReports.Keys.ToArray();
        if (pending.Length == 0)
        {
            return;
        }

        if (!Task.WhenAll(pending).Wait(limit))
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
    /// Why the job did not succeed, or null if it did. When another filter cancels the job, Hangfire still
    /// calls this filter after it, without an exception, although the job never ran.
    /// </summary>
    private static string FailureOf(PerformedContext context)
    {
        if (context.Exception != null)
        {
            return context.Exception.GetBaseException().Message;
        }

        return context.Canceled ? "The job was canceled before it ran" : null;
    }

    /// <summary>
    /// Sends the run event. The returned task completes with the run, or with null if it could not be reported.
    /// </summary>
    private async Task<ITelemetryRun> ReportStart(string monitorKey, string jobId)
    {
        try
        {
            return await _telemetries.StartAsync(monitorKey).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, jobId);
            return null;
        }
    }

    /// <summary>
    /// Sends complete, or fail with the failure, once the run event has been sent, so Cronitor always receives them in order.
    /// </summary>
    private async Task ReportEnd(Task<ITelemetryRun> start, string failure, string jobId)
    {
        try
        {
            using (var run = await start.ConfigureAwait(false))
            {
                if (run == null)
                {
                    return;
                }

                if (failure == null)
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
            _logger.EndNotReported(exception, jobId);
        }
    }
}
