using System;

namespace Cronitor
{
    public class CronitorService : IDisposable
    {
        public MonitorClient MonitorClient;
        public NotificationClient NotificationClient;
        public TelemetryClient TelemetryClient;

        /// <summary>
        /// Configures the instance.
        /// Must be called before any other methods.
        /// </summary>
        public void Configure(string key)
        {
            MonitorClient = new MonitorClient(key);
            NotificationClient = new NotificationClient(key);
            TelemetryClient = new TelemetryClient(key);
        }

        public void Dispose()
        {
            MonitorClient?.Dispose();
            NotificationClient?.Dispose();
            TelemetryClient?.Dispose();
        }
    }
}
