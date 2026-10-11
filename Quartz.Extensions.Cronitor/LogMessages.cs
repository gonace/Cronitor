using Microsoft.Extensions.Logging;
using System;

namespace Quartz;

/// <summary>
/// The package's log messages, generated at compile time by the logging source generator.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Could not report the start of job {JobKey} to Cronitor")]
    internal static partial void StartNotReported(this ILogger logger, Exception exception, JobKey jobKey);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Could not report the end of job {JobKey} to Cronitor")]
    internal static partial void EndNotReported(this ILogger logger, Exception exception, JobKey jobKey);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Warning,
        Message = "Stopped waiting for {Count} job reports to Cronitor while the scheduler shut down")]
    internal static partial void ReportsPendingAtShutdown(this ILogger logger, int count);
}
