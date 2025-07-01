using Cronitor.Clients;
using Cronitor.Internals;

namespace Cronitor
{
    public static class Cronitor
    {
        private static readonly Service Service = new Service();

        /// <summary>
        /// Configures the instance.
        /// Must be called before any other methods.
        /// </summary>
        public static void Configure(string key)
        {
            Service.Configure(key);
        }

        public static IssuesClient Issues => Service.Issues;
        public static IMonitorsClient Monitors => Service.Monitors;
        public static INotificationsClient Notifications => Service.Notifications;
        public static ITelemetriesClient Telemetries => Service.Telemetries;
    }
}
