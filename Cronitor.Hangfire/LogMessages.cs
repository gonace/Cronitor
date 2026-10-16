using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.Hangfire;

/// <summary>
/// The package's log messages, generated at compile time by the logging source generator.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Could not report the start of job {JobId} to Cronitor")]
    internal static partial void StartNotReported(this ILogger logger, Exception exception, string jobId);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Could not report the end of job {JobId} to Cronitor")]
    internal static partial void EndNotReported(this ILogger logger, Exception exception, string jobId);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Warning,
        Message = "Stopped waiting for the end of job {JobId} to be reported to Cronitor; the report continues in the background")]
    internal static partial void EndReportStillPending(this ILogger logger, string jobId);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Information,
        Message = "Not syncing the schedule of monitor {MonitorKey} to Cronitor: {Reason}")]
    internal static partial void ScheduleNotSynced(this ILogger logger, string monitorKey, string reason);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Error,
        Message = "Could not read Hangfire's recurring jobs to sync their schedules to Cronitor")]
    internal static partial void RecurringJobsNotRead(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Warning,
        Message = "Not reporting recurring job {RecurringJobId} to Cronitor: a monitor key can't be blank or longer than 100 characters")]
    internal static partial void RecurringJobIdNotAMonitorKey(this ILogger logger, string recurringJobId);
}
