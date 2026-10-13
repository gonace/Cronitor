using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Cronitor.Internals;

/// <summary>
/// The clients behind the static <see cref="CronitorClient"/>, all created from the same API key.
/// </summary>
internal sealed class Service : IDisposable
{
    public Service(string key)
        : this(key, NullLoggerFactory.Instance)
    {
    }

    public Service(string key, ILoggerFactory loggerFactory)
        : this(new IssuesClient(key), new MonitorsClient(key), new NotificationsClient(key), new TelemetriesClient(key, loggerFactory))
    {
    }

    /// <summary>
    /// Sends every request through the given <see cref="HttpClient"/>.
    /// Intended for tests.
    /// </summary>
    internal Service(string key, HttpClient client)
        : this(key, client, NullLoggerFactory.Instance)
    {
    }

    /// <inheritdoc cref="Service(string, HttpClient)"/>
    internal Service(string key, HttpClient client, ILoggerFactory loggerFactory)
        : this(new IssuesClient(client), new MonitorsClient(client), new NotificationsClient(client), new TelemetriesClient(key, client, loggerFactory))
    {
    }

    private Service(IssuesClient issues, MonitorsClient monitors, NotificationsClient notifications, TelemetriesClient telemetries)
    {
        Issues = issues;
        Monitors = monitors;
        Notifications = notifications;
        Telemetries = telemetries;
    }

    public IssuesClient Issues { get; }
    public MonitorsClient Monitors { get; }
    public NotificationsClient Notifications { get; }
    public TelemetriesClient Telemetries { get; }

    public void Dispose()
    {
        Issues.Dispose();
        Monitors.Dispose();
        Notifications.Dispose();
        Telemetries.Dispose();
    }
}
