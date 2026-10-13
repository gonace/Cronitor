using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// Records every log entry of the shared TickerQ host with its time, so a test can show what TickerQ did while it ran.
/// </summary>
internal sealed class RecordingLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<(DateTime At, string Line)> _entries = new ConcurrentQueue<(DateTime, string)>();

    public ILogger CreateLogger(string categoryName) => new RecordingLogger(this, categoryName);

    /// <summary>
    /// The entries logged at or after the given time, oldest first.
    /// </summary>
    public IEnumerable<string> Since(DateTime from) => _entries.Where(entry => entry.At >= from).Select(entry => entry.Line);

    public void Dispose()
    {
    }

    private void Record(LogLevel level, string category, string message, Exception exception)
    {
        var at = DateTime.UtcNow;
        var error = exception == null ? string.Empty : $" ({exception.GetType().Name}: {exception.Message})";
        _entries.Enqueue((at, $"{at:HH:mm:ss.fff} {level} {category}: {message}{error}"));
    }

    private sealed class RecordingLogger(RecordingLoggerProvider provider, string category) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            provider.Record(logLevel, category, formatter(state, exception), exception);
    }
}
