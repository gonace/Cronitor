using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.DependencyInjection;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Interfaces;
using Xunit;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// Runs cron jobs on a real TickerQ host, set up the way an application would, and checks what is reported to Cronitor.
/// </summary>
[Collection(TickerQHostCollection.Name)]
public class AddCronitorTickerQTests(TickerQHost host)
{
    /// <summary>
    /// How long a test waits for its cron job: until the scheduled second, then up to ten seconds for the job and its reports.
    /// </summary>
    private static readonly TimeSpan Limit = TickerQHost.LeadTime + TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Should_send_run_and_complete_when_a_cron_job_succeeds()
    {
        var telemetries = host.ReportTo(new FakeTelemetriesClient());

        await using (await host.AddCronTicker(nameof(Jobs.SucceedingJob)))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_message_when_a_cron_job_throws()
    {
        var telemetries = host.ReportTo(new FakeTelemetriesClient());

        await using (await host.AddCronTicker(nameof(Jobs.FailingJob)))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run FailingJob", "fail FailingJob: Disk is full" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_complete_once_when_a_retry_of_a_cron_job_succeeds()
    {
        var telemetries = host.ReportTo(new FakeTelemetriesClient());

        await using (await host.AddCronTicker(nameof(Jobs.FailsOnceJob), retries: 1))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run FailsOnceJob", "complete FailsOnceJob" }, telemetries.Sent);
    }

    [Fact]
    public void Should_throw_when_AddCronitorTickerQ_is_called_before_AddTickerQ()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddCronitorTickerQ());

        Assert.Contains("after AddTickerQ()", exception.Message);
    }

    [Fact]
    public void Should_throw_when_AddCronitorTickerQ_is_called_without_configuring_Cronitor()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTickerQ();
        services.AddCronitorTickerQ();

        using (var provider = services.BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<ITickerExecutionTaskHandler>());

            Assert.Contains("ITelemetriesClient", exception.Message);
        }
    }
}

/// <summary>
/// The test jobs. TickerQ's source generator registers them by function name; they have no cron expression of their
/// own, so they only run when a test adds a cron ticker for them.
/// </summary>
public class Jobs
{
    private static readonly ConcurrentDictionary<Guid, int> Attempts = new ConcurrentDictionary<Guid, int>();
    private static readonly TaskCompletionSource<bool> WarmUpRan = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Completes when <see cref="WarmUpJob"/> has run, which <see cref="TickerQHost"/> waits for before the tests start.
    /// </summary>
    public static Task WarmedUp => WarmUpRan.Task;

    [TickerFunction(nameof(WarmUpJob))]
    public Task WarmUpJob(TickerFunctionContext context, CancellationToken cancellationToken)
    {
        WarmUpRan.TrySetResult(true);
        return Task.CompletedTask;
    }

    [TickerFunction(nameof(SucceedingJob))]
    public Task SucceedingJob(TickerFunctionContext context, CancellationToken cancellationToken) => Task.CompletedTask;

    [TickerFunction(nameof(FailingJob))]
    public Task FailingJob(TickerFunctionContext context, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("Disk is full");

    [TickerFunction(nameof(FailsOnceJob))]
    public Task FailsOnceJob(TickerFunctionContext context, CancellationToken cancellationToken) =>
        Attempts.AddOrUpdate(context.Id, 1, (_, attempts) => attempts + 1) == 1
            ? throw new InvalidOperationException("Disk is full")
            : Task.CompletedTask;
}
