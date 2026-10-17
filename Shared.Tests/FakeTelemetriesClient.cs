using Cronitor.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cronitor.Shared.Tests;

/// <summary>
/// Records the telemetry a job run sends, so tests can check it without any HTTP.
/// </summary>
public class FakeTelemetriesClient : ITelemetriesClient
{
    private readonly TaskCompletionSource<bool> _finished = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    public ConcurrentQueue<string> Sent { get; } = new ConcurrentQueue<string>();

    /// <summary>
    /// Completes when a run has ended: reported as completed, failed or abandoned.
    /// </summary>
    public Task Finished => _finished.Task;

    /// <summary>
    /// Makes starting a run throw, like a bug would.
    /// </summary>
    public bool ThrowOnStart { get; set; }

    /// <summary>
    /// Makes completing or failing a run throw, like a bug would.
    /// </summary>
    public bool ThrowOnFinish { get; set; }

    /// <summary>
    /// Holds every start report until this task completes, like a slow Cronitor would.
    /// </summary>
    public Task StartGate { get; set; } = Task.CompletedTask;

    /// <summary>
    /// Holds every complete or fail report until this task completes, like a slow Cronitor would.
    /// </summary>
    public Task FinishGate { get; set; } = Task.CompletedTask;

    public ITelemetryRun Start(MonitorKey monitorKey, string environment = null)
    {
        if (ThrowOnStart)
        {
            throw new InvalidOperationException("Start failed");
        }

        Sent.Enqueue($"run {monitorKey}");
        return new FakeRun(this, monitorKey);
    }

    public async Task<ITelemetryRun> StartAsync(MonitorKey monitorKey, string environment = null)
    {
        await StartGate;
        return Start(monitorKey, environment);
    }

    public bool Run(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> RunAsync(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Complete(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> CompleteAsync(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Fail(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> FailAsync(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Tick(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> TickAsync(MonitorKey monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Ping(Command command) => throw new NotSupportedException();
    public Task<bool> PingAsync(Command command) => throw new NotSupportedException();

    private void Finish(string sent)
    {
        if (ThrowOnFinish)
        {
            throw new InvalidOperationException("Finish failed");
        }

        Sent.Enqueue(sent);
        _finished.TrySetResult(true);
    }

    private sealed class FakeRun : ITelemetryRun
    {
        private readonly FakeTelemetriesClient _client;
        private readonly string _monitorKey;

        public FakeRun(FakeTelemetriesClient client, string monitorKey)
        {
            _client = client;
            _monitorKey = monitorKey;
        }

        public bool Complete(string message = null)
        {
            _client.Finish(message == null ? $"complete {_monitorKey}" : $"complete {_monitorKey}: {message}");
            return true;
        }

        public async Task<bool> CompleteAsync(string message = null)
        {
            await _client.FinishGate;
            return Complete(message);
        }

        public bool Fail(string message = null)
        {
            _client.Finish($"fail {_monitorKey}: {message}");
            return true;
        }

        public async Task<bool> FailAsync(string message = null)
        {
            await _client.FinishGate;
            return Fail(message);
        }

        public bool Abandon(string message)
        {
            _client.Finish($"abandon {_monitorKey}: {message}");
            return true;
        }

        public async Task<bool> AbandonAsync(string message)
        {
            await _client.FinishGate;
            return Abandon(message);
        }

        public void Dispose()
        {
        }
    }
}
