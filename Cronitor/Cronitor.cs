using Cronitor.Commands;
using Cronitor.Internals;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cronitor;

/// <summary>
/// The former name of <see cref="CronitorClient"/>, to which every member forwards. It shares its name with the
/// namespace <c>Cronitor</c>, so outside the library it can only be called as <c>Cronitor.Cronitor</c>.
/// </summary>
[Obsolete("Use CronitorClient, which has the same members. Cronitor shares its name with the namespace Cronitor, so it can only be called as Cronitor.Cronitor.")]
public static class Cronitor
{
    /// <inheritdoc cref="CronitorClient.Configure(string)"/>
    public static void Configure(string key) => CronitorClient.Configure(key);

    /// <inheritdoc cref="CronitorClient.Configure(string, ILoggerFactory)"/>
    public static void Configure(string key, ILoggerFactory loggerFactory) => CronitorClient.Configure(key, loggerFactory);

    internal static void Configure(string key, HttpClient client) => CronitorClient.Configure(key, client);

    public static IssuesClient Issues => CronitorClient.Issues;
    public static IMonitorsClient Monitors => CronitorClient.Monitors;
    public static INotificationsClient Notifications => CronitorClient.Notifications;
    public static ITelemetriesClient Telemetries => CronitorClient.Telemetries;

    /// <inheritdoc cref="ITelemetriesClient.Start"/>
    public static ITelemetryRun Start(MonitorKey monitorKey, string environment = null) =>
        CronitorClient.Start(monitorKey, environment);

    /// <inheritdoc cref="ITelemetriesClient.StartAsync"/>
    public static Task<ITelemetryRun> StartAsync(MonitorKey monitorKey, string environment = null) =>
        CronitorClient.StartAsync(monitorKey, environment);

    public static bool Run(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.Run(monitorKey, message, environment);

    public static Task<bool> RunAsync(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.RunAsync(monitorKey, message, environment);

    public static bool Complete(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.Complete(monitorKey, message, environment);

    public static Task<bool> CompleteAsync(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.CompleteAsync(monitorKey, message, environment);

    public static bool Fail(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.Fail(monitorKey, message, environment);

    public static Task<bool> FailAsync(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.FailAsync(monitorKey, message, environment);

    public static bool Tick(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.Tick(monitorKey, message, environment);

    public static Task<bool> TickAsync(MonitorKey monitorKey, string message = null, string environment = null) =>
        CronitorClient.TickAsync(monitorKey, message, environment);

    public static bool Ping(Command command) =>
        CronitorClient.Ping(command);

    public static Task<bool> PingAsync(Command command) =>
        CronitorClient.PingAsync(command);
}
