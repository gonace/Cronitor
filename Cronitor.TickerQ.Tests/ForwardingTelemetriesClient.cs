using Cronitor.Commands;
using System.Threading.Tasks;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// The telemetry client of the shared TickerQ host. It forwards to the client of the test that is running,
/// so every test gets a fresh <see cref="FakeTelemetriesClient"/> although the host outlives it.
/// </summary>
internal sealed class ForwardingTelemetriesClient : ITelemetriesClient
{
    public ITelemetriesClient Target { get; set; } = new FakeTelemetriesClient();

    public ITelemetryRun Start(MonitorKey monitorKey, string environment = null) => Target.Start(monitorKey, environment);
    public Task<ITelemetryRun> StartAsync(MonitorKey monitorKey, string environment = null) => Target.StartAsync(monitorKey, environment);
    public bool Run(MonitorKey monitorKey, string message = null, string environment = null) => Target.Run(monitorKey, message, environment);
    public Task<bool> RunAsync(MonitorKey monitorKey, string message = null, string environment = null) => Target.RunAsync(monitorKey, message, environment);
    public bool Complete(MonitorKey monitorKey, string message = null, string environment = null) => Target.Complete(monitorKey, message, environment);
    public Task<bool> CompleteAsync(MonitorKey monitorKey, string message = null, string environment = null) => Target.CompleteAsync(monitorKey, message, environment);
    public bool Fail(MonitorKey monitorKey, string message = null, string environment = null) => Target.Fail(monitorKey, message, environment);
    public Task<bool> FailAsync(MonitorKey monitorKey, string message = null, string environment = null) => Target.FailAsync(monitorKey, message, environment);
    public bool Tick(MonitorKey monitorKey, string message = null, string environment = null) => Target.Tick(monitorKey, message, environment);
    public Task<bool> TickAsync(MonitorKey monitorKey, string message = null, string environment = null) => Target.TickAsync(monitorKey, message, environment);
    public bool Ping(Command command) => Target.Ping(command);
    public Task<bool> PingAsync(Command command) => Target.PingAsync(command);
}
