using Cronitor.Clients;
using System;

namespace Cronitor.Internals
{
    internal class Service : IDisposable
    {
        public IssuesClient Issues;
        public MonitorsClient Monitors;
        public NotificationsClient Notifications;
        public TelemetriesClient Telemetries;

        /// <summary>
        /// Configures the instance.
        /// Must be called before any other methods.
        /// </summary>
        public void Configure(string key)
        {
            Issues = new IssuesClient(key);
            Monitors = new MonitorsClient(key);
            Notifications = new NotificationsClient(key);
            Telemetries = new TelemetriesClient(key);
        }

        public void Dispose()
        {
            Issues?.Dispose();
            Monitors?.Dispose();
            Notifications?.Dispose();
            Telemetries?.Dispose();
        }
    }
}
