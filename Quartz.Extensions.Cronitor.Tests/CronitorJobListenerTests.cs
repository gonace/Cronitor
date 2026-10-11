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

    private static Task RunJob<TJob>(string name, FakeTelemetriesClient telemetries) where TJob : IJob =>
        RunJob(JobBuilder.Create<TJob>().WithIdentity(name).Build(), telemetries);

    private static async Task RunJob(IJobDetail job, FakeTelemetriesClient telemetries)
    {
        var properties = new NameValueCollection { ["quartz.scheduler.instanceName"] = Guid.NewGuid().ToString("N") };
        var scheduler = await new StdSchedulerFactory(properties).GetScheduler(TestContext.Current.CancellationToken);
        scheduler.ListenerManager.AddJobListener(new CronitorJobListener(telemetries));
        await scheduler.Start(TestContext.Current.CancellationToken);

        await scheduler.ScheduleJob(job, TriggerBuilder.Create().StartNow().Build(), TestContext.Current.CancellationToken);
        await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));

        await scheduler.Shutdown(true, TestContext.Current.CancellationToken);
    }

    private sealed class SucceedingJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    private sealed class FailingJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => throw new InvalidOperationException("Disk is full");
    }
}
