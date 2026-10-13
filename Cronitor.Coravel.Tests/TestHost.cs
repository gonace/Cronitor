using Coravel;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Coravel.Tests;

/// <summary>
/// A host with a real Coravel scheduler and Cronitor.Coravel, set up the way an application would, reporting to a fake
/// Cronitor client. Coravel's errors are collected in <see cref="Errors"/>.
/// </summary>
public sealed class TestHost : IAsyncDisposable
{
    private readonly IHost _host;

    private TestHost(IHost host)
    {
        _host = host;
    }

    public JobLog Jobs => _host.Services.GetRequiredService<JobLog>();

    public ConcurrentQueue<Exception> Errors { get; } = new ConcurrentQueue<Exception>();

    /// <summary>
    /// Builds and starts a host whose scheduler runs the jobs <paramref name="schedule"/> adds. Without
    /// <paramref name="configure"/>, Cronitor.Coravel is registered with <c>AddCronitorCoravel()</c>.
    /// </summary>
    public static async Task<TestHost> StartAsync(FakeTelemetriesClient telemetries, Action<IScheduler> schedule, Action<IServiceCollection> configure = null)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<ITelemetriesClient>(telemetries);
        builder.Services.AddSingleton<JobLog>();
        builder.Services.AddTransient<SucceedingJob>();
        builder.Services.AddTransient<FailingJob>();
        builder.Services.AddTransient<SlowJob>();
        builder.Services.AddTransient<CancellableJob>();
        builder.Services.AddScheduler();
        (configure ?? (services => services.AddCronitorCoravel()))(builder.Services);

        var testHost = new TestHost(builder.Build());
        testHost._host.Services.UseScheduler(schedule).OnError(testHost.Errors.Enqueue);
        await testHost._host.StartAsync(TestContext.Current.CancellationToken);

        return testHost;
    }

    public Task StopAsync() => _host.StopAsync(TestContext.Current.CancellationToken);

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync(CancellationToken.None);
        _host.Dispose();
    }
}

/// <summary>
/// Records the runs of the test jobs, so a test can wait for them and count them.
/// </summary>
public sealed class JobLog
{
    private readonly ConcurrentDictionary<string, int> _runs = new ConcurrentDictionary<string, int>();
    private readonly TaskCompletionSource<bool> _started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Completes when any job has started.
    /// </summary>
    public Task Started => _started.Task;

    public int RunsOf<TJob>() => _runs.TryGetValue(typeof(TJob).Name, out var runs) ? runs : 0;

    public void Ran<TJob>()
    {
        _runs.AddOrUpdate(typeof(TJob).Name, 1, (_, runs) => runs + 1);
        _started.TrySetResult(true);
    }

    /// <summary>
    /// Waits until the job has run the given number of times, at most ten seconds.
    /// </summary>
    public async Task WaitForRunsOf<TJob>(int runs)
    {
        var limit = DateTime.UtcNow.AddSeconds(10);
        while (RunsOf<TJob>() < runs && DateTime.UtcNow < limit)
        {
            await Task.Delay(50, TestContext.Current.CancellationToken);
        }
    }
}

public sealed class SucceedingJob(JobLog log) : IInvocable
{
    public Task Invoke()
    {
        log.Ran<SucceedingJob>();
        return Task.CompletedTask;
    }
}

public sealed class FailingJob(JobLog log) : IInvocable
{
    public Task Invoke()
    {
        log.Ran<FailingJob>();
        throw new InvalidOperationException("Disk is full");
    }
}

/// <summary>
/// Runs for longer than the second between its runs, so with <c>PreventOverlapping</c> Coravel skips the next run.
/// </summary>
public sealed class SlowJob(JobLog log) : IInvocable
{
    public async Task Invoke()
    {
        log.Ran<SlowJob>();
        await Task.Delay(TimeSpan.FromSeconds(1.5));
    }
}

/// <summary>
/// Runs until Coravel cancels it, which Coravel does when the application stops.
/// </summary>
public sealed class CancellableJob(JobLog log) : IInvocable, ICancellableInvocable
{
    public CancellationToken CancellationToken { get; set; }

    public async Task Invoke()
    {
        log.Ran<CancellableJob>();
        await Task.Delay(Timeout.Infinite, CancellationToken);
    }
}
