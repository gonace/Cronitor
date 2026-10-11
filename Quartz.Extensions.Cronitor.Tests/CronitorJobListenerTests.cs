using Microsoft.Extensions.Logging;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace Quartz.Extensions.Cronitor.Tests;

/// <summary>
/// Runs jobs on a real Quartz scheduler and checks what the listener reports to Cronitor.
/// </summary>
[Collection(StaticQuartzCollection.Name)]
public class CronitorJobListenerTests
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

    private static Task RunJob<TJob>(string name, FakeTelemetriesClient telemetries) where TJob : IJob =>
        RunJob(JobBuilder.Create<TJob>().WithIdentity(name).Build(), telemetries);

    private static Task RunJob(IJobDetail job, FakeTelemetriesClient telemetries) =>
        RunJob(job, new CronitorJobListener(telemetries), telemetries.Finished);

    private static async Task RunJob(IJobDetail job, IJobListener listener, Task done)
    {
        var properties = new NameValueCollection { ["quartz.scheduler.instanceName"] = Guid.NewGuid().ToString("N") };
        var scheduler = await new StdSchedulerFactory(properties).GetScheduler(TestContext.Current.CancellationToken);
        scheduler.ListenerManager.AddJobListener(listener);
        await scheduler.Start(TestContext.Current.CancellationToken);

        await scheduler.ScheduleJob(job, TriggerBuilder.Create().StartNow().Build(), TestContext.Current.CancellationToken);
        await Task.WhenAny(done, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        await scheduler.Shutdown(true, TestContext.Current.CancellationToken);
    }

    private sealed class SucceedingJob : IJob
    {
        private static TaskCompletionSource<bool> _ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Ran => _ran.Task;

        public static void Reset() => _ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Execute(IJobExecutionContext context)
        {
            _ran.TrySetResult(true);
            return Task.CompletedTask;
        }
    }

    private sealed class FailingJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => throw new InvalidOperationException("Disk is full");
    }
}
