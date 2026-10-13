using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Interfaces;
using TickerQ.Utilities.Models;
using Xunit;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// Wraps a fake TickerQ handler that ends each job with a chosen status, and checks what is reported to Cronitor.
/// </summary>
public class CronitorExecutionTaskHandlerTests
{
    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(10);

    [Theory]
    [InlineData(TickerStatus.Done)]
    [InlineData(TickerStatus.DueDone)]
    public async Task Should_send_run_and_complete_when_TickerQ_ends_a_cron_job_as_done(TickerStatus status)
    {
        var telemetries = new FakeTelemetriesClient();

        await Run(new CronitorExecutionTaskHandler(EndsWith(status), telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_a_cron_job_whose_function_name_has_a_slash_to_the_key_with_the_slash_replaced_by_a_dash()
    {
        var telemetries = new FakeTelemetriesClient();
        var cronJob = new InternalFunctionContext { FunctionName = "reports/daily", Type = TickerType.CronTickerOccurrence, TickerId = Guid.NewGuid() };

        await Run(new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Done), telemetries), cronJob, telemetries);

        Assert.Equal(new[] { "run reports-daily", "complete reports-daily" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_complete_without_the_error_of_a_failed_attempt_when_a_retry_succeeds()
    {
        var telemetries = new FakeTelemetriesClient();
        var inner = EndsWith(TickerStatus.Done, """{"Message":"Disk is full","StackTrace":"at BackupJob"}""");

        await Run(new CronitorExecutionTaskHandler(inner, telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_exception_message_when_TickerQ_ends_a_cron_job_as_Failed()
    {
        var telemetries = new FakeTelemetriesClient();
        var inner = EndsWith(TickerStatus.Failed, """{"Message":"Disk is full","StackTrace":"at BackupJob"}""");

        await Run(new CronitorExecutionTaskHandler(inner, telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Disk is full" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_fail_with_the_exception_details_as_they_are_when_they_are_not_JSON()
    {
        var telemetries = new FakeTelemetriesClient();

        await Run(new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Failed, "Disk is full"), telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Disk is full" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_complete_with_the_reason_when_a_cron_job_skips_itself()
    {
        var telemetries = new FakeTelemetriesClient();

        await Run(new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Skipped, "Nothing to back up"), telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup: Nothing to back up" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_when_TickerQ_ends_a_cron_job_as_Cancelled()
    {
        var telemetries = new FakeTelemetriesClient();

        await Run(new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Cancelled), telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was cancelled" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_status_when_TickerQ_ends_a_cron_job_with_an_unexpected_status()
    {
        var telemetries = new FakeTelemetriesClient();

        await Run(new CronitorExecutionTaskHandler(EndsWith(TickerStatus.InProgress), telemetries), CronJob(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: TickerQ ended the job with status InProgress" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_and_still_throw_when_TickerQ_s_own_handler_throws()
    {
        var telemetries = new FakeTelemetriesClient();
        var handler = new CronitorExecutionTaskHandler(new FakeTickerQHandler((_, _) => Task.FromException(new InvalidOperationException("Store is down"))), telemetries);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken));
        await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));

        Assert.Equal("Store is down", exception.Message);
        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Store is down" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_and_still_throw_when_TickerQ_s_own_handler_throws_synchronously()
    {
        var telemetries = new FakeTelemetriesClient();
        var handler = new CronitorExecutionTaskHandler(new FakeTickerQHandler((_, _) => throw new InvalidOperationException("Store is down")), telemetries);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken));
        await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));

        Assert.Equal("Store is down", exception.Message);
        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Store is down" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_run_but_not_report_a_one_off_time_ticker()
    {
        var telemetries = new FakeTelemetriesClient();
        var inner = EndsWith(TickerStatus.Done);
        var handler = new CronitorExecutionTaskHandler(inner, telemetries);
        var timeTicker = new InternalFunctionContext { FunctionName = "send-invoice", Type = TickerType.TimeTicker, TickerId = Guid.NewGuid() };

        await handler.ExecuteTaskAsync(timeTicker, false, TestContext.Current.CancellationToken);
        await handler.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);

        Assert.Equal(1, inner.Calls);
        Assert.Empty(telemetries.Sent);
    }

    [Fact]
    public async Task Should_still_run_the_job_and_log_the_error_when_the_start_report_fails()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnStart = true };
        var logger = new FakeLogger<CronitorExecutionTaskHandler>();
        var inner = EndsWith(TickerStatus.Done);
        var handler = new CronitorExecutionTaskHandler(inner, telemetries, logger);

        await handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken);
        await handler.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);

        Assert.Equal(1, inner.Calls);
        Assert.Empty(telemetries.Sent);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Start failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_log_the_error_when_the_end_report_fails()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnFinish = true };
        var logger = new FakeLogger<CronitorExecutionTaskHandler>();
        var handler = new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Done), telemetries, logger);

        await handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken);
        await handler.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);

        Assert.Equal(new[] { "run nightly-backup" }, telemetries.Sent);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Finish failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_run_the_job_without_waiting_for_the_start_to_be_reported()
    {
        var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { StartGate = startGate.Task };
        var inner = EndsWith(TickerStatus.Done);
        var handler = new CronitorExecutionTaskHandler(inner, telemetries);

        var finishedWhileStartWasHeld = await FinishesWithin(handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken), TimeSpan.FromSeconds(5));
        var sentWhileStartWasHeld = telemetries.Sent.ToArray();
        startGate.SetResult(true);
        await handler.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);

        Assert.True(finishedWhileStartWasHeld, "The handler made the job wait for the start report.");
        Assert.Equal(1, inner.Calls);
        Assert.Empty(sentWhileStartWasHeld);
        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_finish_the_job_without_waiting_for_the_end_to_be_reported()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var handler = new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Done), telemetries);

        var finishedWhileEndWasHeld = await FinishesWithin(handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken), TimeSpan.FromSeconds(5));
        finishGate.SetResult(true);
        await handler.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);

        Assert.True(finishedWhileEndWasHeld, "The handler made the job wait for the end report.");
        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_wait_for_a_job_still_running_and_its_end_report_when_waiting_for_pending_reports()
    {
        var jobGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient();
        var handler = new CronitorExecutionTaskHandler(new FakeTickerQHandler(async (context, _) =>
        {
            await jobGate.Task;
            context.Status = TickerStatus.Cancelled;
        }), telemetries);

        var execution = handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken);
        _ = Task.Delay(TimeSpan.FromMilliseconds(300), TestContext.Current.CancellationToken).ContinueWith(_ => jobGate.SetResult(true), TaskScheduler.Default);
        await handler.WaitForPendingReportsAsync(Limit, TestContext.Current.CancellationToken);
        var sentAfterWaiting = telemetries.Sent.ToArray();
        await execution;

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was cancelled" }, sentAfterWaiting);
    }

    [Fact]
    public async Task Should_stop_waiting_for_pending_reports_after_the_limit()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var logger = new FakeLogger<CronitorExecutionTaskHandler>();
        var handler = new CronitorExecutionTaskHandler(EndsWith(TickerStatus.Done), telemetries, logger);
        await handler.ExecuteTaskAsync(CronJob(), false, TestContext.Current.CancellationToken);

        var waiting = handler.WaitForPendingReportsAsync(TimeSpan.FromMilliseconds(200), TestContext.Current.CancellationToken);
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

    private static InternalFunctionContext CronJob() =>
        new InternalFunctionContext { FunctionName = "nightly-backup", Type = TickerType.CronTickerOccurrence, TickerId = Guid.NewGuid() };

    private static FakeTickerQHandler EndsWith(TickerStatus status, string exceptionDetails = null) =>
        new FakeTickerQHandler((context, _) =>
        {
            context.Status = status;
            context.ExceptionDetails = exceptionDetails;
            return Task.CompletedTask;
        });

    private static async Task Run(CronitorExecutionTaskHandler handler, InternalFunctionContext context, FakeTelemetriesClient telemetries)
    {
        await handler.ExecuteTaskAsync(context, false, TestContext.Current.CancellationToken);
        await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
    }

    private static async Task<bool> FinishesWithin(Task task, TimeSpan limit) =>
        await Task.WhenAny(task, Task.Delay(limit, TestContext.Current.CancellationToken)) == task;
}
