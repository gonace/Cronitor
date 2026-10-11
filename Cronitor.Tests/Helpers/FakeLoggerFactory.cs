using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cronitor.Tests.Helpers
{
    /// <summary>
    /// Records every log entry, from every logger it creates, so tests can assert on them.
    /// </summary>
    public class FakeLoggerFactory : ILoggerFactory
    {
        private readonly List<LogEntry> _entries = new();

        public IReadOnlyList<LogEntry> Entries => _entries;

        public ILogger CreateLogger(string categoryName) => new FakeLogger(categoryName, _entries);

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }

        public class LogEntry
        {
            public LogEntry(string category, LogLevel level, string message, Exception exception, IReadOnlyList<KeyValuePair<string, object>> values)
            {
                Category = category;
                Level = level;
                Message = message;
                Exception = exception;
                Values = values;
            }

            public string Category { get; }
            public LogLevel Level { get; }
            public string Message { get; }
            public Exception Exception { get; }
            public IReadOnlyList<KeyValuePair<string, object>> Values { get; }
        }

        private class FakeLogger : ILogger
        {
            private readonly string _category;
            private readonly List<LogEntry> _entries;

            public FakeLogger(string category, List<LogEntry> entries)
            {
                _category = category;
                _entries = entries;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var values = state as IEnumerable<KeyValuePair<string, object>> ?? Enumerable.Empty<KeyValuePair<string, object>>();
                _entries.Add(new LogEntry(_category, logLevel, formatter(state, exception), exception, values.ToList()));
            }
        }
    }
}
