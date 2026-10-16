using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.TickerQ;

/// <summary>
/// The package's log messages, generated at compile time by the logging source generator.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Could not report the start of cron job {FunctionName} ({TickerId}) to Cronitor")]
    internal static partial void StartNotReported(this ILogger logger, Exception exception, string functionName, Guid tickerId);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Could not report the end of cron job {FunctionName} ({TickerId}) to Cronitor")]
    internal static partial void EndNotReported(this ILogger logger, Exception exception, string functionName, Guid tickerId);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Warning,
        Message = "Stopped waiting for {Count} cron job reports to Cronitor while the application stopped")]
    internal static partial void ReportsPendingAtShutdown(this ILogger logger, int count);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Information,
        Message = "Not syncing the schedule of monitor {MonitorKey} to Cronitor: {Reason}")]
    internal static partial void ScheduleNotSynced(this ILogger logger, string monitorKey, string reason);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Error,
        Message = "Could not read TickerQ's cron tickers to sync their schedules to Cronitor")]
    internal static partial void CronTickersNotRead(this ILogger logger, Exception exception);
}
