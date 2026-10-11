using Cronitor;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Hangfire;

/// <summary>
/// Reports every recurring job Hangfire runs to Cronitor: run when the job starts, then complete when it
/// succeeds or fail when it throws. The recurring job's ID is used as the monitor key; other jobs are not reported.
/// Reporting never affects the job: Hangfire skips a job when a filter throws before it, and marks a successful
/// job as failed when a filter throws after it, so every failure here is logged instead of thrown.
/// </summary>
public class CronitorJobFilter : IServerFilter
{
    private const string RunKey = "Cronitor.Run";
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
            ReportStart(context);
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
            ReportEnd(context);
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.BackgroundJob.Id);
        }
    }

    private void ReportStart(PerformingContext context)
    {
        var monitorKey = context.GetJobParameter<string>(RecurringJobIdParameter);
        if (string.IsNullOrWhiteSpace(monitorKey))
        {
            return;
        }

        context.Items[RunKey] = _telemetries.Start(monitorKey);
    }

    private static void ReportEnd(PerformedContext context)
    {
        if (!context.Items.TryGetValue(RunKey, out var value) || value is not ITelemetryRun run)
        {
            return;
        }

        using (run)
        {
            if (context.Exception == null)
            {
                run.Complete();
            }
            else
            {
                run.Fail(context.Exception.GetBaseException().Message);
            }
        }
    }
}
