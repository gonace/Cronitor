using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Quartz.Tests;

/// <summary>
/// Runs jobs on a real Quartz scheduler and checks what the listener reports to Cronitor.
/// </summary>
[Collection(StaticQuartzCollection.Name)]
public partial class CronitorJobListenerTests
{
    [Fact]
    public async Task Should_send_run_and_complete_when_a_job_succeeds()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob<SucceedingJob>("nightly-backup", telemetries);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_message_when_a_job_throws()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob<FailingJob>("nightly-backup", telemetries);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Disk is full" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_to_the_monitor_key_chosen_with_WithCronitorMonitorKey()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("backup").WithCronitorMonitorKey("nightly-backup").Build(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_a_job_named_with_a_slash_to_the_key_with_the_slash_replaced_by_a_dash()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("reports/daily").Build(), telemetries);

        Assert.Equal(new[] { "run reports-daily", "complete reports-daily" }, telemetries.Sent);
    }

    [Fact]
    public void Should_throw_when_WithCronitorMonitorKey_is_given_a_key_with_a_slash()
    {
        var job = JobBuilder.Create<SucceedingJob>();

        var exception = Assert.Throws<ArgumentException>(() => job.WithCronitorMonitorKey("reports/daily"));

        Assert.Contains("/", exception.Message);
    }

    [Fact]
    public async Task Should_report_to_the_job_class_name_when_Quartz_generated_the_job_name()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob(JobBuilder.Create<SucceedingJob>().Build(), telemetries);

        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_to_the_monitor_key_chosen_with_WithCronitorMonitorKey_when_Quartz_generated_the_job_name()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithCronitorMonitorKey("nightly-backup").Build(), telemetries);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    /// <summary>
    /// Quartz records nothing that tells a generated name from one set to a GUID on purpose, so both are treated as generated.
    /// </summary>
    [Fact]
    public async Task Should_report_a_job_named_with_a_GUID_under_its_class_name()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("3f2c8a1e-5b7d-4c9e-a6f0-1d2b3c4e5f60").Build(), telemetries);

        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_a_job_named_with_a_GUID_to_the_monitor_key_chosen_with_WithCronitorMonitorKey()
    {
        var telemetries = new FakeTelemetriesClient();
        var job = JobBuilder.Create<SucceedingJob>().WithIdentity("3f2c8a1e-5b7d-4c9e-a6f0-1d2b3c4e5f60").WithCronitorMonitorKey("sync-3f2c8a1e").Build();

        await RunJob(job, telemetries);

        Assert.Equal(new[] { "run sync-3f2c8a1e", "complete sync-3f2c8a1e" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_not_report_a_job_whose_name_is_too_long_for_a_monitor_key_and_log_a_warning()
    {
        var telemetries = new FakeTelemetriesClient();
        var logger = new FakeLogger<CronitorJobListener>();
        SucceedingJob.Reset();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity(new string('x', 101)).Build(), new CronitorJobListener(telemetries, logger), SucceedingJob.Ran);

        Assert.True(SucceedingJob.Ran.IsCompleted);
        Assert.Empty(telemetries.Sent);
        Assert.Equal(LogLevel.Warning, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public async Task Should_not_report_a_job_whose_chosen_key_is_invalid_and_log_a_warning()
    {
        var telemetries = new FakeTelemetriesClient();
        var logger = new FakeLogger<CronitorJobListener>();
        SucceedingJob.Reset();
        var job = JobBuilder.Create<SucceedingJob>().WithIdentity("payroll").UsingJobData(CronitorJobBuilderExtensions.MonitorKey, "reports/payroll").Build();

        await RunJob(job, new CronitorJobListener(telemetries, logger), SucceedingJob.Ran);

        Assert.True(SucceedingJob.Ran.IsCompleted);
        Assert.Empty(telemetries.Sent);
        Assert.Equal(LogLevel.Warning, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public async Task Should_still_run_the_job_and_log_the_error_when_the_listener_fails_before_it()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnStart = true };
        var logger = new FakeLogger<CronitorJobListener>();
        SucceedingJob.Reset();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("nightly-backup").Build(), new CronitorJobListener(telemetries, logger), SucceedingJob.Ran);

        Assert.True(SucceedingJob.Ran.IsCompleted);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Start failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_log_the_error_when_the_listener_fails_after_the_job()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnFinish = true };
        var logger = new FakeLogger<CronitorJobListener>();
        SucceedingJob.Reset();

        await RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("nightly-backup").Build(), new CronitorJobListener(telemetries, logger), SucceedingJob.Ran);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Finish failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_send_run_and_fail_when_the_job_is_interrupted()
    {
        var telemetries = new FakeTelemetriesClient();
        InterruptibleJob.Reset();
        var scheduler = await NewScheduler();
        AddJobListener(scheduler, new CronitorJobListener(telemetries));
        await scheduler.Start(TestContext.Current.CancellationToken);

        var job = JobBuilder.Create<InterruptibleJob>().WithIdentity("nightly-backup").Build();
        await scheduler.ScheduleJob(job, TriggerBuilder.Create().StartNow().Build(), cancellationToken: TestContext.Current.CancellationToken);
        await Task.WhenAny(InterruptibleJob.Started, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
        await scheduler.Interrupt(job.Key, TestContext.Current.CancellationToken);
        await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
        await scheduler.Shutdown(true, TestContext.Current.CancellationToken);

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: The job was interrupted" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_run_the_job_without_waiting_for_the_start_to_be_reported()
    {
        var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { StartGate = startGate.Task };
        SucceedingJob.Reset();

        var run = RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("nightly-backup").Build(), new CronitorJobListener(telemetries), SucceedingJob.Ran);
        var finishedWhileStartWasHeld = await FinishesWithin(run, TimeSpan.FromSeconds(5));
        var sentWhileStartWasHeld = telemetries.Sent.ToArray();
        startGate.SetResult(true);
        await run;
        await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        Assert.True(finishedWhileStartWasHeld, "The listener made the job wait for the start report.");
        Assert.Empty(sentWhileStartWasHeld);
        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_finish_the_job_without_waiting_for_the_end_to_be_reported()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        SucceedingJob.Reset();

        var run = RunJob(JobBuilder.Create<SucceedingJob>().WithIdentity("nightly-backup").Build(), new CronitorJobListener(telemetries), SucceedingJob.Ran);
        var finishedWhileEndWasHeld = await FinishesWithin(run, TimeSpan.FromSeconds(5));
        finishGate.SetResult(true);
        await run;
        await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        Assert.True(finishedWhileEndWasHeld, "The listener made the job wait for the end report.");
        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    /// <summary>
    /// Whether the job run, listener included, finishes within the limit. A held report is released by the caller afterwards.
    /// </summary>
    [Fact]
    public async Task Should_send_pending_end_reports_before_the_scheduler_shuts_down()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var listener = new CronitorJobListener(telemetries);
        var scheduler = await StartAndRunJob(listener, new CronitorSchedulerListener(listener));

        _ = Task.Delay(TimeSpan.FromMilliseconds(500), TestContext.Current.CancellationToken).ContinueWith(_ => finishGate.SetResult(true), TaskScheduler.Default);
        await scheduler.Shutdown(true, TestContext.Current.CancellationToken);

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_stop_waiting_for_pending_end_reports_after_the_shutdown_limit()
    {
        var finishGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { FinishGate = finishGate.Task };
        var logger = new FakeLogger<CronitorJobListener>();
        var listener = new CronitorJobListener(telemetries, logger);
        var scheduler = await StartAndRunJob(listener, new CronitorSchedulerListener(listener, TimeSpan.FromMilliseconds(200)));

        var shutdown = ShutdownAsTask(scheduler);
        var shutDownWithinLimit = await FinishesWithin(shutdown, TimeSpan.FromSeconds(5));
        var sentAtShutdown = telemetries.Sent.ToArray();
        finishGate.SetResult(true);
        await shutdown;

        Assert.True(shutDownWithinLimit, "Shutdown kept waiting for the report beyond the limit.");
        Assert.Equal(new[] { "run nightly-backup" }, sentAtShutdown);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
    }

    /// <summary>
    /// Starts a scheduler with the given listeners, runs a succeeding job and waits until the job has run.
    /// </summary>
    private static async Task<IScheduler> StartAndRunJob(IJobListener jobListener, ISchedulerListener schedulerListener)
    {
        SucceedingJob.Reset();
        var scheduler = await NewScheduler();
        AddJobListener(scheduler, jobListener);
        scheduler.ListenerManager.AddSchedulerListener(schedulerListener);
        await scheduler.Start(TestContext.Current.CancellationToken);

        await scheduler.ScheduleJob(JobBuilder.Create<SucceedingJob>().WithIdentity("nightly-backup").Build(), TriggerBuilder.Create().StartNow().Build(), cancellationToken: TestContext.Current.CancellationToken);
        await Task.WhenAny(SucceedingJob.Ran, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        return scheduler;
    }

    private static async Task<bool> FinishesWithin(Task run, TimeSpan limit) =>
        await Task.WhenAny(run, Task.Delay(limit, TestContext.Current.CancellationToken)) == run;

    private static Task RunJob<TJob>(string name, FakeTelemetriesClient telemetries) where TJob : IJob =>
        RunJob(JobBuilder.Create<TJob>().WithIdentity(name).Build(), telemetries);

    private static Task RunJob(IJobDetail job, FakeTelemetriesClient telemetries) =>
        RunJob(job, new CronitorJobListener(telemetries), telemetries.Finished);

    private static async Task RunJob(IJobDetail job, IJobListener listener, Task done)
    {
        var scheduler = await NewScheduler();
        AddJobListener(scheduler, listener);
        await scheduler.Start(TestContext.Current.CancellationToken);

        await scheduler.ScheduleJob(job, TriggerBuilder.Create().StartNow().Build(), cancellationToken: TestContext.Current.CancellationToken);
        await Task.WhenAny(done, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        await scheduler.Shutdown(true, TestContext.Current.CancellationToken);
    }
}
