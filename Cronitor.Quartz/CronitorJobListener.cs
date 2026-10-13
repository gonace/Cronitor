using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Quartz;

/// <summary>
/// Reports every job Quartz runs to Cronitor: run when the job starts, then complete when it
/// succeeds or fail when it throws. The job's name is used as the monitor key, unless the job
/// chooses one with <see cref="CronitorJobBuilderExtensions.WithCronitorMonitorKey"/>.
/// A job without a name of its own, which Quartz names with a new GUID on every start,
/// reports under its class name instead.
/// Reporting never affects the job: Quartz waits for a listener before and after every job, and
/// skips a job when a listener throws before it. So reports are sent in the background, without
/// the job waiting for Cronitor, and every failure is logged instead of thrown.
/// </summary>
public class CronitorJobListener : IJobListener
{
    private const string StartKey = "Cronitor.Start";

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorJobListener> _logger;
    private readonly ConcurrentDictionary<Task, bool> _pendingReports = new ConcurrentDictionary<Task, bool>();

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
                Track(ReportEnd(start, FailureOf(context, jobException), context.JobDetail.Key));
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.JobDetail.Key);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Waits for the end reports still being sent, at most for the given time, so they are not lost when the
    /// application stops. <see cref="CronitorSchedulerListener"/> calls it when the scheduler shuts down.
    /// </summary>
    internal async Task WaitForPendingReports(TimeSpan limit, CancellationToken cancellationToken)
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
    /// Why the job did not succeed, or null if it did. Quartz passes no exception for a job it interrupted,
    /// even when the job threw because of the interrupt, so an interrupt is checked separately.
    /// </summary>
    private static string FailureOf(IJobExecutionContext context, JobExecutionException jobException)
    {
        if (jobException != null)
        {
            return jobException.GetBaseException().Message;
        }

        return context.CancellationToken.IsCancellationRequested ? "The job was interrupted" : null;
    }

    /// <summary>
    /// Sends complete, or fail with the failure, once the run event has been sent, so Cronitor always receives them in order.
    /// </summary>
    private async Task ReportEnd(Task<ITelemetryRun> start, string failure, JobKey jobKey)
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
            _logger.EndNotReported(exception, jobKey);
        }
    }

    private static string MonitorKeyOf(IJobExecutionContext context)
    {
        if (context.MergedJobDataMap.TryGetString(CronitorJobBuilderExtensions.MonitorKey, out var monitorKey) && !string.IsNullOrWhiteSpace(monitorKey))
        {
            return monitorKey;
        }

        var name = context.JobDetail.Key.Name;
        return IsGeneratedName(name) ? context.JobDetail.JobType.Name : name;
    }

    /// <summary>
    /// Whether Quartz made up the job's name. A job built without an identity gets a new GUID as its name on every
    /// start, which would report to a new monitor each time, so its class name is used instead. Quartz records nothing
    /// that tells a generated name from one set to a GUID on purpose, so every GUID name counts as generated; such a
    /// job keeps a monitor of its own with <see cref="CronitorJobBuilderExtensions.WithCronitorMonitorKey"/>.
    /// </summary>
    private static bool IsGeneratedName(string name) => Guid.TryParseExact(name, "D", out _);
}
