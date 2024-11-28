using Cronitor.Clients;
using Cronitor.Exceptions;
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

        public static IssuesClient Issues
        {
            get
            {
                if (Service.Issues == null)
                    throw new NotConfiguredException();

                return Service.Issues;
            }
        }

        public static IMonitorsClient Monitors
        {
            get
            {
                if (Service.Monitors == null)
                    throw new NotConfiguredException();

                return Service.Monitors;
            }
        }

        public static INotificationsClient Notifications
        {
            get
            {
                if (Service.Notifications == null)
                    throw new NotConfiguredException();

                return Service.Notifications;
            }
        }

        public static ITelemetriesClient Telemetries
        {
            get
            {
                if (Service.Telemetries == null)
                    throw new NotConfiguredException();

                return Service.Telemetries;
            }
        }

        public static bool Configured =>
            Service.Issues != null &&
            Service.Monitors != null &&
            Service.Notifications != null &&
            Service.Telemetries != null;
    }
}
