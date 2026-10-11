using Cronitor;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz;

/// <summary>
/// Reports every job Quartz runs to Cronitor: run when the job starts, then complete when it
/// succeeds or fail when it throws. The job's name is used as the monitor key, unless the job
/// chooses one with <see cref="CronitorJobBuilderExtensions.WithCronitorMonitorKey"/>.
/// Telemetry never breaks a job: if Cronitor cannot be reached, the job still runs as normal.
/// </summary>
public class CronitorJobListener : IJobListener
{
    private const string RunKey = "Cronitor.Run";

    private readonly ITelemetriesClient _telemetries;

    public CronitorJobListener(ITelemetriesClient telemetries)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
    }

    public string Name => "Cronitor";

    public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var run = await _telemetries.StartAsync(MonitorKeyOf(context)).ConfigureAwait(false);
        context.Put(RunKey, run);
    }

    private static string MonitorKeyOf(IJobExecutionContext context)
    {
        return context.MergedJobDataMap.TryGetString(CronitorJobBuilderExtensions.MonitorKey, out var monitorKey) && !string.IsNullOrWhiteSpace(monitorKey)
            ? monitorKey
            : context.JobDetail.Key.Name;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
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
}
