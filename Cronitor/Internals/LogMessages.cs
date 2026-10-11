using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.Internals
{
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
    }
}
