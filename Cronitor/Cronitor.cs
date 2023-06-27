﻿using Cronitor.Clients;
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

        public static IMonitorsClient Monitor => CronitorService.Monitors;
        public static INotificationsClient Notification => CronitorService.Notifications;
        public static ITelemetriesClient Telemetries => CronitorService.Telemetries;
    }
}
