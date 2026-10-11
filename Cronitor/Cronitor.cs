using Cronitor.Commands;
using Cronitor.Exceptions;
using Cronitor.Internals;
using System.Threading.Tasks;

namespace Cronitor
{
    public static class Cronitor
    {
        private static Service _service;

        /// <summary>
        /// Configures the instance.
        /// Must be called before any other methods.
        /// </summary>
        public static void Configure(string key)
        {
            _service = new Service(key);
        }

        internal static void Configure(string key, HttpClient client)
        {
            _service = new Service(key, client);
        }

        public static IssuesClient Issues => Service.Issues;
        public static IMonitorsClient Monitors => Service.Monitors;
        public static INotificationsClient Notifications => Service.Notifications;
        public static ITelemetriesClient Telemetries => Service.Telemetries;

        /// <inheritdoc cref="ITelemetriesClient.Start"/>
        public static ITelemetryRun Start(string monitorKey, string environment = null) =>
            Telemetries.Start(monitorKey, environment);

        /// <inheritdoc cref="ITelemetriesClient.StartAsync"/>
        public static Task<ITelemetryRun> StartAsync(string monitorKey, string environment = null) =>
            Telemetries.StartAsync(monitorKey, environment);

        public static void Run(string monitorKey, string message = null, string environment = null) =>
            Telemetries.Run(monitorKey, message, environment);

        public static Task RunAsync(string monitorKey, string message = null, string environment = null) =>
            Telemetries.RunAsync(monitorKey, message, environment);

        public static void Complete(string monitorKey, string message = null, string environment = null) =>
            Telemetries.Complete(monitorKey, message, environment);

        public static Task CompleteAsync(string monitorKey, string message = null, string environment = null) =>
            Telemetries.CompleteAsync(monitorKey, message, environment);

        public static void Fail(string monitorKey, string message = null, string environment = null) =>
            Telemetries.Fail(monitorKey, message, environment);

        public static Task FailAsync(string monitorKey, string message = null, string environment = null) =>
            Telemetries.FailAsync(monitorKey, message, environment);

        public static void Tick(string monitorKey, string message = null, string environment = null) =>
            Telemetries.Tick(monitorKey, message, environment);

        public static Task TickAsync(string monitorKey, string message = null, string environment = null) =>
            Telemetries.TickAsync(monitorKey, message, environment);

        public static void Ping(Command command) =>
            Telemetries.Ping(command);

        public static Task PingAsync(Command command) =>
            Telemetries.PingAsync(command);

        private static Service Service => _service ?? throw new NotConfiguredException();
    }
}
