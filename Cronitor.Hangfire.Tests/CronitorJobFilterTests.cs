using Hangfire;
using Hangfire.Common;
using Hangfire.InMemory;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Hangfire.Tests;

/// <summary>
/// Runs jobs on a real Hangfire server, with in-memory storage, and checks what the filter reports to Cronitor.
/// </summary>
public class CronitorJobFilterTests
{
    [Fact]
    public async Task Should_send_run_and_complete_when_a_recurring_job_succeeds()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries));

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public void Should_read_the_job_s_parameters_from_what_Hangfire_fetched_with_the_job_without_asking_storage()
    {
        var telemetries = new FakeTelemetriesClient();
        var filter = new CronitorJobFilter(telemetries);
        var parameters = new Dictionary<string, string> { ["RecurringJobId"] = "\"nightly-backup\"" };
        var job = new BackgroundJob("1", Job.FromExpression(() => TestJobs.Succeed()), DateTime.UtcNow, parameters);
        var context = new PerformContext(new InMemoryStorage(), new UnreachableStorageConnection(), job, new JobCancellationToken(false));

        filter.OnPerforming(new PerformingContext(context));
        filter.OnPerformed(new PerformedContext(context, null, false, null));

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_message_when_a_recurring_job_throws()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunRecurringJob("nightly-backup", () => TestJobs.Throw(), new CronitorJobFilter(telemetries));

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Disk is full" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_abandon_the_run_when_the_server_stops_while_a_recurring_job_runs()
    {
        var telemetries = new FakeTelemetriesClient();
        var storage = new InMemoryStorage();
        TestJobs.ResetWaiting();

        using (var server = TestServer.Start(storage, new JobFilterCollection { new CronitorJobFilter(telemetries) }))
        {
            StartRecurringJob(storage, "nightly-backup", () => TestJobs.WaitForCancellation(CancellationToken.None));
            await TestServer.WaitUntil(() => TestJobs.Waiting.IsCompleted);
            server.SendStop();
            await TestServer.WaitUntil(() => telemetries.Sent.Count == 2);
        }

        Assert.Equal(new[] { "run nightly-backup", "abandon nightly-backup: The service went offline" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_abandon_the_run_when_a_running_recurring_job_is_deleted()
    {
        var telemetries = new FakeTelemetriesClient();
        var storage = new InMemoryStorage();
        TestJobs.ResetWaiting();

        using (TestServer.Start(storage, new JobFilterCollection { new CronitorJobFilter(telemetries) }))
        {
            StartRecurringJob(storage, "nightly-backup", () => TestJobs.WaitForCancellation(CancellationToken.None));
            await TestServer.WaitUntil(() => TestJobs.Waiting.IsCompleted);
            new BackgroundJobClient(storage).Delete(storage.GetMonitoringApi().ProcessingJobs(0, 1).Single().Key);
            await TestServer.WaitUntil(() => telemetries.Sent.Count == 2);
        }

        Assert.Equal(new[] { "run nightly-backup", "abandon nightly-backup: The job was aborted" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_fail_when_a_recurring_job_cancels_itself_while_the_server_keeps_running()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunRecurringJob("nightly-backup", () => TestJobs.ThrowOperationCanceled(), new CronitorJobFilter(telemetries));

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Gave up" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_add_the_attempt_to_the_fail_message_when_Hangfire_retries_a_recurring_job()
    {
        var telemetries = new FakeTelemetriesClient();
        var filters = new JobFilterCollection
        {
            new CronitorJobFilter(telemetries),
            new AutomaticRetryAttribute { Attempts = 1, DelaysInSeconds = new[] { 1 } },
        };
        var storage = new InMemoryStorage();

        using (TestServer.Start(storage, filters))
        {
            var recurringJobs = new RecurringJobManager(storage);
            recurringJobs.AddOrUpdate("nightly-backup", () => TestJobs.Throw(), Cron.Never());
            recurringJobs.Trigger("nightly-backup");

            await TestServer.WaitUntil(() => telemetries.Sent.Count == 4);
        }

        Assert.Equal(
            new[] { "run nightly-backup", "fail nightly-backup: Disk is full", "run nightly-backup", "fail nightly-backup: Disk is full (attempt 2)" },
            telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_a_recurring_job_whose_ID_has_a_slash_to_the_key_with_the_slash_replaced_by_a_dash()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunRecurringJob("reports/daily", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries));

        Assert.Equal(new[] { "run reports-daily", "complete reports-daily" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_not_report_a_job_that_is_not_recurring()
    {
        var telemetries = new FakeTelemetriesClient();
        var storage = new InMemoryStorage();

        using (TestServer.Start(storage, new JobFilterCollection { new CronitorJobFilter(telemetries) }))
        {
            new BackgroundJobClient(storage).Enqueue(() => TestJobs.Succeed());
            await TestServer.WaitUntil(() => storage.GetMonitoringApi().SucceededListCount() == 1);
        }

        Assert.Empty(telemetries.Sent);
    }

    [Fact]
    public async Task Should_not_report_a_recurring_job_whose_ID_is_too_long_for_a_monitor_key_and_log_a_warning()
    {
        var telemetries = new FakeTelemetriesClient();
        var logger = new FakeLogger<CronitorJobFilter>();

        var storage = await RunRecurringJob(new string('x', 101), () => TestJobs.Succeed(), new CronitorJobFilter(telemetries, logger));

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        Assert.Empty(telemetries.Sent);
        Assert.Equal(LogLevel.Warning, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public async Task Should_still_run_the_job_and_log_the_error_when_the_filter_fails_before_it()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnStart = true };
        var logger = new FakeLogger<CronitorJobFilter>();

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries, logger));

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Start failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_keep_the_job_succeeded_and_log_the_error_when_the_filter_fails_after_it()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnFinish = true };
        var logger = new FakeLogger<CronitorJobFilter>();

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries, logger));

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        Assert.Equal(0, storage.GetMonitoringApi().FailedCount());
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Finish failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_run_the_job_without_waiting_for_the_start_to_be_reported()
    {
        var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { StartGate = startGate.Task };

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries));
        var sentWhileStartWasHeld = telemetries.Sent.ToArray();
        startGate.SetResult(true);
        await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        Assert.Empty(sentWhileStartWasHeld);
        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_the_end_before_Hangfire_finishes_the_job()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        _ = Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken).ContinueWith(_ => finishGate.SetResult(true), TaskScheduler.Default);

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries));
        var sentWhenTheJobFinished = telemetries.Sent.ToArray();

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, sentWhenTheJobFinished);
    }

    [Fact]
    public async Task Should_stop_waiting_for_the_end_report_after_the_limit()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var logger = new FakeLogger<CronitorJobFilter>();

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries, logger, TimeSpan.FromMilliseconds(200)));
        var sentWhenTheJobFinished = telemetries.Sent.ToArray();
        finishGate.SetResult(true);

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        Assert.Equal(new[] { "run nightly-backup" }, sentWhenTheJobFinished);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
    }

    [Fact]
    public async Task Should_send_run_and_fail_when_another_filter_cancels_the_job()
    {
        var telemetries = new FakeTelemetriesClient();
        var filters = new JobFilterCollection();
        filters.Add(new CronitorJobFilter(telemetries), 0);
        filters.Add(new CancellingFilter(), 1);
        var storage = new InMemoryStorage();

        using (TestServer.Start(storage, filters))
        {
            var recurringJobs = new RecurringJobManager(storage);
            recurringJobs.AddOrUpdate("nightly-backup", () => TestJobs.Succeed(), Cron.Never());
            recurringJobs.Trigger("nightly-backup");

            await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was canceled before it ran" }, telemetries.Sent);
    }

    private static Task<InMemoryStorage> RunRecurringJob(string recurringJobId, Expression<Action> job, CronitorJobFilter filter) =>
        TestServer.RunRecurringJob(recurringJobId, job, new JobFilterCollection { filter });

    private static void StartRecurringJob(JobStorage storage, string recurringJobId, Expression<Func<Task>> job)
    {
        var recurringJobs = new RecurringJobManager(storage);
        recurringJobs.AddOrUpdate(recurringJobId, job, Cron.Never());
        recurringJobs.Trigger(recurringJobId);
    }

    /// <summary>
    /// Cancels every job before it runs, like a filter that decides a job should not run now.
    /// </summary>
    private sealed class CancellingFilter : IServerFilter
    {
        public void OnPerforming(PerformingContext context) => context.Canceled = true;

        public void OnPerformed(PerformedContext context)
        {
        }
    }
}
