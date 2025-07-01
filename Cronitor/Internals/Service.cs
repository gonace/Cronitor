using Cronitor.Clients;
using Cronitor.Exceptions;
using System;

namespace Cronitor.Internals
{
    internal class Service : IDisposable
    {
        private IssuesClient _issues;
        public IssuesClient Issues
        {
            get
            {
                if (_issues == null)
                {
                    throw new NotConfiguredException();
                }

                return _issues;
            }
            private set => _issues = value;
        }

        private MonitorsClient _monitors;
        public MonitorsClient Monitors
        {
            get
            {
                if (_monitors == null)
                {
                    throw new NotConfiguredException();
                }

                return _monitors;
            }
            private set => _monitors = value;
        }

        private NotificationsClient _notifications;
        public NotificationsClient Notifications
        {
            get
            {
                if (_notifications == null)
                {
                    throw new NotConfiguredException();
                }

                return _notifications;
            }
            private set => _notifications = value;
        }

        private TelemetriesClient _telemetries;
        public TelemetriesClient Telemetries
        {
            get
            {
                if (_notifications == null)
                {
                    throw new NotConfiguredException();
                }

                return _telemetries;
            }
            private set => _telemetries = value;
        }

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
