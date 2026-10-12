using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Cronitor.Shared.Tests;

/// <summary>
/// Records every log entry, so tests can assert on them.
/// </summary>
public class FakeLogger<T> : ILogger<T>
{
    public ConcurrentQueue<(LogLevel Level, Exception Exception)> Entries { get; } = new ConcurrentQueue<(LogLevel, Exception)>();

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Entries.Enqueue((logLevel, exception));
    }
}
