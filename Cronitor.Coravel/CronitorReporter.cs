using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
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

    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorReporter> _logger;

    public CronitorReporter(ITelemetriesClient telemetries, ILogger<CronitorReporter> logger = null)
    {
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorReporter>.Instance;
    }

    /// <summary>
    /// Runs the job and reports it to the monitor with the given key. The returned task is the job's own, so Coravel
    /// still sees the job's exception.
    /// </summary>
    internal Task RunAsync(string monitorKey, Func<Task> job)
    {
        var start = ReportStartAsync(monitorKey);
        var execution = ExecuteAsync(job);

        _ = ReportEndAsync(start, execution, monitorKey);

        return execution;
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
    private async Task<ITelemetryRun> ReportStartAsync(string monitorKey)
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
    /// Sends complete or fail once the job has ended and the run event has been sent, so Cronitor always receives them in order.
    /// </summary>
    private async Task ReportEndAsync(Task<ITelemetryRun> start, Task execution, string monitorKey)
    {
        try
        {
            var failure = await FailureOfAsync(execution).ConfigureAwait(false);
            using (var run = await start.ConfigureAwait(false))
            {
                if (run == null)
                {
                    return;
                }

                await (failure == null ? run.CompleteAsync() : run.FailAsync(failure)).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, monitorKey);
        }
    }

    /// <summary>
    /// Why the job did not succeed, or null if it did. Coravel cancels jobs when the application stops.
    /// </summary>
    private static async Task<string> FailureOfAsync(Task execution)
    {
        try
        {
            await execution.ConfigureAwait(false);
            return null;
        }
        catch (OperationCanceledException)
        {
            return CancelledMessage;
        }
        catch (Exception exception)
        {
            return exception.GetBaseException().Message;
        }
    }
}
