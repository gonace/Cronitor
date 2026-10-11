using Cronitor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz;

/// <summary>
/// Reports every job Quartz runs to Cronitor: run when the job starts, then complete when it
/// succeeds or fail when it throws. The job's name is used as the monitor key, unless the job
/// chooses one with <see cref="CronitorJobBuilderExtensions.WithCronitorMonitorKey"/>.
/// Reporting never affects the job: Quartz waits for a listener before and after every job, and
/// skips a job when a listener throws before it. So reports are sent in the background, without
/// the job waiting for Cronitor, and every failure is logged instead of thrown.
/// </summary>
public class CronitorJobListener : IJobListener
{
    private const string StartKey = "Cronitor.Start";

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorJobListener> _logger;

    public CronitorJobListener(ITelemetriesClient telemetries, ILogger<CronitorJobListener> logger = null)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorJobListener>.Instance;
    }

    public string Name => "Cronitor";

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            context.Put(StartKey, ReportStart(context));
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, context.JobDetail.Key);
        }

        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
    {
        try
        {
            if (context.Get(StartKey) is Task<ITelemetryRun> start)
            {
                _ = ReportEnd(start, jobException, context.JobDetail.Key);
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.JobDetail.Key);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends the run event. The returned task completes with the run, or with null if it could not be reported.
    /// </summary>
    private async Task<ITelemetryRun> ReportStart(IJobExecutionContext context)
    {
        try
        {
            return await _telemetries.StartAsync(MonitorKeyOf(context)).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, context.JobDetail.Key);
            return null;
        }
    }

    /// <summary>
    /// Sends complete or fail once the run event has been sent, so Cronitor always receives them in order.
    /// </summary>
    private async Task ReportEnd(Task<ITelemetryRun> start, JobExecutionException jobException, JobKey jobKey)
    {
        try
        {
            using (var run = await start.ConfigureAwait(false))
            {
                if (run == null)
                {
                    return;
                }

                if (jobException == null)
                {
                    await run.CompleteAsync().ConfigureAwait(false);
                }
                else
                {
                    await run.FailAsync(jobException.GetBaseException().Message).ConfigureAwait(false);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, jobKey);
        }
    }

    private static string MonitorKeyOf(IJobExecutionContext context)
    {
        return context.MergedJobDataMap.TryGetString(CronitorJobBuilderExtensions.MonitorKey, out var monitorKey) && !string.IsNullOrWhiteSpace(monitorKey)
            ? monitorKey
            : context.JobDetail.Key.Name;
    }
}
