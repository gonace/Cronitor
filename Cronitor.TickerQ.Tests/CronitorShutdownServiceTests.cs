using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Models;
using Xunit;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// Stops a real host in which a stand-in for TickerQ cancels its running job without waiting for it, as TickerQ does,
/// and checks that the job's end still reaches Cronitor before the host has stopped.
/// </summary>
public class CronitorShutdownServiceTests
{
    [Fact]
    public async Task Should_send_the_end_of_a_job_cancelled_by_the_stopping_application_before_it_has_stopped()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var jobCancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var handler = new CronitorExecutionTaskHandler(new FakeTickerQHandler(async (context, cancellationToken) =>
        {
            await RunUntilCancelled(context, cancellationToken);
            jobCancelled.SetResult(true);
        }), telemetries);
        var scheduler = new CancelsJobsWhenStopping(handler);

        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddHostedService(_ => scheduler);
        builder.Services.AddHostedService(_ => new CronitorShutdownService(handler));

        bool stoppedWhileReportWasHeld;
        using (var host = builder.Build())
        {
            await host.StartAsync(TestContext.Current.CancellationToken);
            await Task.WhenAny(scheduler.JobStarted, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

            // The end report stays held until the host has been seen still stopping after the job was cancelled, so
            // the host only finishes stopping in time if it does not wait for the report.
            var stop = host.StopAsync(TestContext.Current.CancellationToken);
            await Task.WhenAny(jobCancelled.Task, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
            stoppedWhileReportWasHeld = await FinishesWithin(stop, TimeSpan.FromMilliseconds(500));
            finishGate.SetResult(true);
            await stop;
        }

        Assert.True(jobCancelled.Task.IsCompleted, "The stand-in did not cancel the job.");
        Assert.False(stoppedWhileReportWasHeld, "The host stopped without waiting for the end report.");
        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was cancelled" }, telemetries.Sent);
    }

    private static async Task<bool> FinishesWithin(Task task, TimeSpan limit) =>
        await Task.WhenAny(task, Task.Delay(limit, TestContext.Current.CancellationToken)) == task;

    private static async Task RunUntilCancelled(InternalFunctionContext context, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            context.Status = TickerStatus.Cancelled;
        }
    }

    /// <summary>
    /// Runs one cron job when started and, like TickerQ, cancels it when stopping without waiting for it to end.
    /// </summary>
    private sealed class CancelsJobsWhenStopping(CronitorExecutionTaskHandler handler) : IHostedService
    {
        private readonly CancellationTokenSource _stopping = new CancellationTokenSource();
        private readonly TaskCompletionSource<bool> _jobStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task JobStarted => _jobStarted.Task;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var context = new InternalFunctionContext { FunctionName = "nightly-backup", Type = TickerType.CronTickerOccurrence, TickerId = Guid.NewGuid() };
            _ = handler.ExecuteTaskAsync(context, false, _stopping.Token);
            _jobStarted.SetResult(true);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopping.Cancel();
            return Task.CompletedTask;
        }
    }
}
