using Cronitor.Exceptions;
using System;

namespace Cronitor.Internals
{
    internal class Service : IDisposable
    {
        public Service()
        {
        }

        public Service(string key)
        {
            Configure(key);
        }

        /// <summary>
        /// Sends every request through the given <see cref="HttpClient"/>.
        /// Intended for tests.
        /// </summary>
        internal Service(string key, HttpClient client)
        {
            Issues = new IssuesClient(client);
            Monitors = new MonitorsClient(client);
            Notifications = new NotificationsClient(client);
            Telemetries = new TelemetriesClient(key, client);
        }

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
                if (_telemetries == null)
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

        /// <summary>
        /// Configures the telemetries client with the given <see cref="HttpClient"/>.
        /// Intended for tests.
        /// </summary>
        internal void Configure(string key, HttpClient client)
        {
            Telemetries = new TelemetriesClient(key, client);
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
