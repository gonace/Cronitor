using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Coravel.Tests;

/// <summary>
/// Checks that the end of a job still being reported when the application stops reaches Cronitor, within a limit.
/// </summary>
public class ShutdownTests
{
    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Should_send_the_end_report_still_pending_before_the_host_has_stopped()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        bool stoppedWhileReportWasHeld;

        await using (var host = await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<SucceedingJob>().EverySecond().Once()))
        {
            await host.Jobs.WaitForRunsOf<SucceedingJob>(1);

            var stop = host.StopAsync();
            stoppedWhileReportWasHeld = await FinishesWithin(stop, TimeSpan.FromMilliseconds(500));
            finishGate.SetResult(true);
            await stop;
        }

        Assert.False(stoppedWhileReportWasHeld, "The host stopped without waiting for the end report.");
        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_stop_waiting_for_pending_reports_after_the_limit()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var logger = new FakeLogger<CronitorReporter>();
        var reporter = new CronitorReporter(telemetries, logger);
        await reporter.RunAsync("nightly-backup", () => Task.CompletedTask, CancellationToken.None);

        var waiting = reporter.WaitForPendingReportsAsync(TimeSpan.FromMilliseconds(200), TestContext.Current.CancellationToken);
        // 200 ms requested; a second allows for a slow test host but fails a wait that ignores the limit.
        var stoppedWithinLimit = await FinishesWithin(waiting, TimeSpan.FromSeconds(1));
        var sentWhenStopped = telemetries.Sent.ToArray();
        finishGate.SetResult(true);
        await waiting;

        Assert.True(stoppedWithinLimit, "Waiting for the report went beyond the limit.");
        Assert.Equal(new[] { "run nightly-backup" }, sentWhenStopped);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
    }

    [Fact]
    public async Task Should_send_run_and_fail_when_the_application_only_stops_after_a_cancelled_job_has_ended()
    {
        var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { StartGate = startGate.Task };
        using (var applicationStopping = new CancellationTokenSource())
        {
            var reporter = new CronitorReporter(telemetries, applicationStopping: applicationStopping.Token);

            var cancelledByCoravel = new CancellationToken(canceled: true);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reporter.RunAsync("nightly-backup", () => Task.FromCanceled(cancelledByCoravel), cancelledByCoravel));
            await applicationStopping.CancelAsync();
            startGate.SetResult(true);
            await reporter.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);
        }

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was cancelled" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_when_a_job_cancels_itself_while_the_application_stops()
    {
        var telemetries = new FakeTelemetriesClient();
        var reporter = new CronitorReporter(telemetries, applicationStopping: new CancellationToken(canceled: true));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reporter.RunAsync("nightly-backup", () => Task.FromCanceled(new CancellationToken(canceled: true)), CancellationToken.None));
        await reporter.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was cancelled" }, telemetries.Sent);
    }

    private static async Task<bool> FinishesWithin(Task task, TimeSpan limit) =>
        await Task.WhenAny(task, Task.Delay(limit, TestContext.Current.CancellationToken)) == task;
}
