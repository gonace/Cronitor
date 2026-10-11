using System;

namespace Cronitor.Internals
{
    /// <summary>
    /// The clients behind the static <see cref="Cronitor"/> class, all created from the same API key.
    /// </summary>
    internal sealed class Service : IDisposable
    {
        public Service(string key)
            : this(new IssuesClient(key), new MonitorsClient(key), new NotificationsClient(key), new TelemetriesClient(key))
        {
        }

        /// <summary>
        /// Sends every request through the given <see cref="HttpClient"/>.
        /// Intended for tests.
        /// </summary>
        internal Service(string key, HttpClient client)
            : this(new IssuesClient(client), new MonitorsClient(client), new NotificationsClient(client), new TelemetriesClient(key, client))
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
}
