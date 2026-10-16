using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Interfaces;
using TickerQ.Utilities.Models;

namespace Cronitor.TickerQ;

/// <summary>
/// Wraps TickerQ's own <see cref="ITickerExecutionTaskHandler"/> and reports every cron job it runs to Cronitor:
/// run when the job starts, then complete or fail from the status TickerQ ends the job with. The job's function
/// name is used as the monitor key; one-off time tickers are not reported. TickerQ runs all retries of an occurrence
/// within one call, so each occurrence is reported once. Reporting never affects the job: reports are sent in the
/// background, and every failure is logged instead of thrown.
/// </summary>
internal sealed class CronitorExecutionTaskHandler : ITickerExecutionTaskHandler
{
    private const string CancelledMessage = "The job was cancelled";

    private readonly ITickerExecutionTaskHandler _inner;
    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CronitorExecutionTaskHandler> _logger;
    private readonly ConcurrentDictionary<Task, bool> _pendingReports = new ConcurrentDictionary<Task, bool>();

    public CronitorExecutionTaskHandler(ITickerExecutionTaskHandler inner, ITelemetriesClient telemetries, ILogger<CronitorExecutionTaskHandler> logger = null)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _telemetries = telemetries ?? throw new ArgumentNullException(nameof(telemetries));
        _logger = logger ?? NullLogger<CronitorExecutionTaskHandler>.Instance;
    }

    public Task ExecuteTaskAsync(InternalFunctionContext context, bool isDue, CancellationToken cancellationToken = default)
    {
        if (context.Type != TickerType.CronTickerOccurrence)
        {
            return _inner.ExecuteTaskAsync(context, isDue, cancellationToken);
        }

        var start = ReportStartAsync(context);
        var execution = ExecuteAsync(context, isDue, cancellationToken);

        // Tracked from the start: TickerQ cancels running jobs when the application stops without waiting for them,
        // so a job may only end after the application has begun waiting for the reports.
        Track(ReportEndAsync(start, execution, context));

        return execution;
    }

    /// <summary>
    /// Waits for the jobs still running and the reports still being sent, at most for the given time, so they are
    /// not lost when the application stops. <see cref="CronitorShutdownService"/> calls it once TickerQ has stopped.
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

    /// <summary>
    /// Runs TickerQ's own handler, turning an exception it throws before returning a task into a faulted task,
    /// so the end of the run is still reported and the exception still reaches TickerQ when it awaits the job.
    /// </summary>
    private Task ExecuteAsync(InternalFunctionContext context, bool isDue, CancellationToken cancellationToken)
    {
        try
        {
            return _inner.ExecuteTaskAsync(context, isDue, cancellationToken);
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
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
    private async Task<ITelemetryRun> ReportStartAsync(InternalFunctionContext context)
    {
        if (!MonitorKey.TryFromName(context.FunctionName, out var monitorKey))
        {
            _logger.FunctionNameNotAMonitorKey(context.FunctionName);
            return null;
        }

        try
        {
            return await _telemetries.StartAsync(monitorKey).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.StartNotReported(exception, context.FunctionName, context.TickerId);
            return null;
        }
    }

    /// <summary>
    /// Sends complete or fail once the job has ended and the run event has been sent, so Cronitor always receives them in order.
    /// </summary>
    private async Task ReportEndAsync(Task<ITelemetryRun> start, Task execution, InternalFunctionContext context)
    {
        try
        {
            var crash = await CrashOfAsync(execution).ConfigureAwait(false);
            using (var run = await start.ConfigureAwait(false))
            {
                if (run == null)
                {
                    return;
                }

                await (crash == null ? SendEndAsync(run, context) : run.FailAsync(crash)).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            _logger.EndNotReported(exception, context.FunctionName, context.TickerId);
        }
    }

    /// <summary>
    /// The message of the exception TickerQ's own handler threw, or null if it returned. It does not throw when a job
    /// fails, so this only covers a fault in TickerQ itself.
    /// </summary>
    private static async Task<string> CrashOfAsync(Task execution)
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

    /// <summary>
    /// Reports the status TickerQ ended the job with. A job that skips itself, by throwing TickerQ's
    /// <c>TerminateExecutionException</c>, ended on purpose, so it completes with its reason.
    /// </summary>
    private static Task<bool> SendEndAsync(ITelemetryRun run, InternalFunctionContext context) => context.Status switch
    {
        TickerStatus.Done or TickerStatus.DueDone => run.CompleteAsync(),
        TickerStatus.Skipped => run.CompleteAsync(context.ExceptionDetails),
        TickerStatus.Failed => run.FailAsync(ErrorMessageOf(context.ExceptionDetails)),
        TickerStatus.Cancelled => run.FailAsync(CancelledMessage),
        var status => run.FailAsync($"TickerQ ended the job with status {status}"),
    };

    /// <summary>
    /// TickerQ stores a failed job's exception as JSON, <c>{"Message": …, "StackTrace": …}</c>; anything else is used as it is.
    /// </summary>
    private static string ErrorMessageOf(string exceptionDetails)
    {
        if (string.IsNullOrEmpty(exceptionDetails))
        {
            return exceptionDetails;
        }

        try
        {
            using (var details = JsonDocument.Parse(exceptionDetails))
            {
                if (details.RootElement.ValueKind == JsonValueKind.Object
                    && details.RootElement.TryGetProperty("Message", out var message)
                    && message.ValueKind == JsonValueKind.String)
                {
                    return message.GetString();
                }
            }
        }
        catch (JsonException)
        {
        }

        return exceptionDetails;
    }
}
