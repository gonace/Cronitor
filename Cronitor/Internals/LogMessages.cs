using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.Internals;

/// <summary>
/// The library's log messages, generated at compile time by the logging source generator.
/// Never log telemetry URLs: they contain the API key.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Could not deliver {EventName} event for monitor {MonitorKey} to Cronitor")]
    internal static partial void TelemetryNotDelivered(this ILogger logger, Exception exception, string eventName, string monitorKey);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Synced the schedules of {Count} monitors to Cronitor")]
    internal static partial void SchedulesSynced(this ILogger logger, int count);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Warning,
        Message = "Cronitor rejected the schedules of monitor {MonitorKey}: {Reason}")]
    internal static partial void ScheduleRejected(this ILogger logger, string monitorKey, string reason);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Could not sync the schedules of {Count} monitors to Cronitor")]
    internal static partial void SchedulesNotSynced(this ILogger logger, Exception exception, int count);
}
