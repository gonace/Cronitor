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

    public ITelemetryRun Start(string monitorKey, string environment = null) => Target.Start(monitorKey, environment);
    public Task<ITelemetryRun> StartAsync(string monitorKey, string environment = null) => Target.StartAsync(monitorKey, environment);
    public bool Run(string monitorKey, string message = null, string environment = null) => Target.Run(monitorKey, message, environment);
    public Task<bool> RunAsync(string monitorKey, string message = null, string environment = null) => Target.RunAsync(monitorKey, message, environment);
    public bool Complete(string monitorKey, string message = null, string environment = null) => Target.Complete(monitorKey, message, environment);
    public Task<bool> CompleteAsync(string monitorKey, string message = null, string environment = null) => Target.CompleteAsync(monitorKey, message, environment);
    public bool Fail(string monitorKey, string message = null, string environment = null) => Target.Fail(monitorKey, message, environment);
    public Task<bool> FailAsync(string monitorKey, string message = null, string environment = null) => Target.FailAsync(monitorKey, message, environment);
    public bool Tick(string monitorKey, string message = null, string environment = null) => Target.Tick(monitorKey, message, environment);
    public Task<bool> TickAsync(string monitorKey, string message = null, string environment = null) => Target.TickAsync(monitorKey, message, environment);
    public bool Ping(Command command) => Target.Ping(command);
    public Task<bool> PingAsync(Command command) => Target.PingAsync(command);
}
