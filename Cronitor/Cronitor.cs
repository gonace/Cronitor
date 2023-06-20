using Cronitor.Clients;
using Cronitor.Internals;

namespace Cronitor
{
    public static class Cronitor
    {
        private static readonly CronitorService CronitorService = new CronitorService();

        /// <summary>
        /// Configures the instance.
        /// Must be called before any other methods.
        /// </summary>
        public static void Configure(string key)
        {
            CronitorService.Configure(key);
        }

        public static MonitorClient Monitor => CronitorService.MonitorClient;
        public static NotificationClient Notification => CronitorService.NotificationClient;
        public static TelemetryClient Telemetry => CronitorService.TelemetryClient;
    }
}
