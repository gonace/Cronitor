using Cronitor;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
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

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorJobFilter> _logger;

    public CronitorJobFilter(ITelemetriesClient telemetries, ILogger<CronitorJobFilter> logger = null)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorJobFilter>.Instance;
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
                _ = ReportEnd(start, FailureOf(context), context.BackgroundJob.Id);
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.BackgroundJob.Id);
        }
    }

    /// <summary>
    /// Why the job did not succeed, or null if it did.
    /// </summary>
    private static string FailureOf(PerformedContext context) => context.Exception?.GetBaseException().Message;

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
