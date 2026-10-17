using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Cronitor.Hangfire;

/// <summary>
/// Reports every recurring job Hangfire runs to Cronitor: run when the job starts, then complete when it
/// succeeds or fail when it throws. The recurring job's ID is used as the monitor key; other jobs are not reported.
/// Each attempt is reported as its own run, so when Hangfire retries a failed job, the retry's fail message
/// ends with its attempt, such as "(attempt 2)".
/// Reporting never affects the job: Hangfire runs filters on the job's worker, skips a job when a filter throws
/// before it, and marks a successful job as failed when a filter throws after it. So the run is reported in the
/// background, the worker waits at most five seconds for the end to be reported, and every failure is logged
/// instead of thrown. If the end report takes longer, the worker moves on and the report continues in the
/// background, so a report still pending when the application stops can be lost.
/// </summary>
public class CronitorJobFilter : IServerFilter
{
    private const string StartKey = "Cronitor.Start";
    private const string RecurringJobIdParameter = "RecurringJobId";
    private const string RetryCountParameter = "RetryCount";

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorJobFilter> _logger;
    private readonly TimeSpan _endReportLimit;

    public CronitorJobFilter(ITelemetriesClient telemetries, ILogger<CronitorJobFilter> logger = null)
        : this(telemetries, logger, TimeSpan.FromSeconds(5))
    {
    }

    internal CronitorJobFilter(ITelemetriesClient telemetries, ILogger<CronitorJobFilter> logger, TimeSpan endReportLimit)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorJobFilter>.Instance;
        _endReportLimit = endReportLimit;
    }

    public void OnPerforming(PerformingContext context)
    {
        try
        {
            var recurringJobId = context.GetJobParameter<string>(RecurringJobIdParameter, allowStale: true);
            if (string.IsNullOrWhiteSpace(recurringJobId))
            {
                return;
            }

            if (!MonitorKey.TryFromName(recurringJobId, out var monitorKey))
            {
                _logger.RecurringJobIdNotAMonitorKey(recurringJobId);
                return;
            }

            context.Items[StartKey] = ReportStartAsync(monitorKey, context.BackgroundJob.Id);
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
            if (context.Items.TryGetValue(StartKey, out var value) && value is Task<ITelemetryRun> start
                && !ReportEndAsync(start, FailureOf(context), context.BackgroundJob.Id).Wait(_endReportLimit))
            {
                _logger.EndReportStillPending(context.BackgroundJob.Id);
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.BackgroundJob.Id);
        }
    }

    /// <summary>
    /// Why the job did not succeed, with the attempt when it was a retry, or null if it succeeded.
    /// </summary>
    private static string FailureOf(PerformedContext context)
    {
        var failure = ReasonOf(context);
        var retries = context.GetJobParameter<int>(RetryCountParameter, allowStale: true);

        return failure != null && retries > 0 ? $"{failure} (attempt {retries + 1})" : failure;
    }

    /// <summary>
    /// Why the job did not succeed, or null if it did. When another filter cancels the job, Hangfire still
    /// calls this filter after it, without an exception, although the job never ran.
    /// </summary>
    private static string ReasonOf(PerformedContext context)
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
    private async Task<ITelemetryRun> ReportStartAsync(MonitorKey monitorKey, string jobId)
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
    private async Task ReportEndAsync(Task<ITelemetryRun> start, string failure, string jobId)
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
