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
/// Reporting never affects the job: Quartz skips a job when a listener throws before it,
/// so every failure here is logged instead of thrown.
/// </summary>
public class CronitorJobListener : IJobListener
{
    private const string RunKey = "Cronitor.Run";

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorJobListener> _logger;

    public CronitorJobListener(ITelemetriesClient telemetries, ILogger<CronitorJobListener> logger = null)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorJobListener>.Instance;
    }

    public string Name => "Cronitor";

    public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var run = await _telemetries.StartAsync(MonitorKeyOf(context)).ConfigureAwait(false);
            context.Put(RunKey, run);
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, context.JobDetail.Key);
        }
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
    {
        try
        {
            await ReportEnd(context, jobException).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.JobDetail.Key);
        }
    }

    private static async Task ReportEnd(IJobExecutionContext context, JobExecutionException jobException)
    {
        if (context.Get(RunKey) is not ITelemetryRun run)
        {
            return;
        }

        using (run)
        {
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

    private static string MonitorKeyOf(IJobExecutionContext context)
    {
        return context.MergedJobDataMap.TryGetString(CronitorJobBuilderExtensions.MonitorKey, out var monitorKey) && !string.IsNullOrWhiteSpace(monitorKey)
            ? monitorKey
            : context.JobDetail.Key.Name;
    }
}
