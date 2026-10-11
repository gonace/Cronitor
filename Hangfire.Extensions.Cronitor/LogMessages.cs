using Microsoft.Extensions.Logging;
using System;

namespace Hangfire;

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
}
