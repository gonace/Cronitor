using Cronitor;
using Cronitor.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quartz.Extensions.Cronitor.Tests;

/// <summary>
/// Records the telemetry a job run sends, so tests can check it without any HTTP.
/// </summary>
public class FakeTelemetriesClient : ITelemetriesClient
{
    private readonly TaskCompletionSource<bool> _finished = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    public ConcurrentQueue<string> Sent { get; } = new ConcurrentQueue<string>();

    /// <summary>
    /// Completes when a run has been reported as completed or failed.
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

    public ITelemetryRun Start(string monitorKey, string environment = null)
    {
        if (ThrowOnStart)
        {
            throw new InvalidOperationException("Start failed");
        }

        Sent.Enqueue($"run {monitorKey}");
        return new FakeRun(this, monitorKey);
    }

    public Task<ITelemetryRun> StartAsync(string monitorKey, string environment = null) => Task.FromResult(Start(monitorKey, environment));

    public bool Run(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> RunAsync(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Complete(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> CompleteAsync(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Fail(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> FailAsync(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public bool Tick(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
    public Task<bool> TickAsync(string monitorKey, string message = null, string environment = null) => throw new NotSupportedException();
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
            _client.Finish($"complete {_monitorKey}");
            return true;
        }

        public Task<bool> CompleteAsync(string message = null) => Task.FromResult(Complete(message));

        public bool Fail(string message = null)
        {
            _client.Finish($"fail {_monitorKey}: {message}");
            return true;
        }

        public Task<bool> FailAsync(string message = null) => Task.FromResult(Fail(message));

        public void Dispose()
        {
        }
    }
}
