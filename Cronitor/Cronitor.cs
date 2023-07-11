using Cronitor.Clients;
using Cronitor.Exceptions;
using Cronitor.Internals;

namespace Cronitor
{
    public static class Cronitor
    {
        private static readonly Service Service = new Service();

        public static bool IsConfigured { get; private set; }

        /// <summary>
        /// Configures the instance.
        /// Must be called before any other methods.
        /// </summary>
        public static void Configure(string key)
        {
            Service.Configure(key);
            IsConfigured = true;
        }

        public static IMonitorsClient Monitor
        {
            get
            {
                if (!IsConfigured || Service.Monitors == null)
                    throw new NotConfiguredException();

                return Service.Monitors;
            }
        }

        public static INotificationsClient Notification
        {
            get
            {
                if (!IsConfigured || Service.Notifications == null)
                    throw new NotConfiguredException();

                return Service.Notifications;
            }
        }

        public static ITelemetriesClient Telemetries
        {
            get
            {
                if (!IsConfigured || Service.Telemetries == null)
                    throw new NotConfiguredException();

                return Service.Telemetries;
            }
        }
    }
}
