using Cronitor;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Quartz.Extensions.Cronitor.Tests;

/// <summary>
/// Registers the listener through Quartz's dependency injection, the way an application would.
/// </summary>
[Collection(StaticQuartzCollection.Name)]
public class UseCronitorTests : IDisposable
{
    /// <summary>
    /// <c>AddQuartz</c> points Quartz's global logging at the test's logger factory, which is disposed
    /// with the service provider, so the next test must not inherit it.
    /// </summary>
    public void Dispose() => LogProvider.SetCurrentLogProvider(null);

    [Fact]
    public async Task Should_report_jobs_when_UseCronitor_is_called()
    {
        var telemetries = new FakeTelemetriesClient();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ITelemetriesClient>(telemetries);
        services.AddQuartz(q =>
        {
            q.SchedulerName = Guid.NewGuid().ToString("N");
            q.UseCronitor();
        });

        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = await provider.GetRequiredService<ISchedulerFactory>().GetScheduler(TestContext.Current.CancellationToken);
            await scheduler.Start(TestContext.Current.CancellationToken);
            await scheduler.ScheduleJob(JobBuilder.Create<SucceedingJob>().WithIdentity("nightly-backup").Build(), TriggerBuilder.Create().StartNow().Build(), TestContext.Current.CancellationToken);
            await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
            await scheduler.Shutdown(true, TestContext.Current.CancellationToken);
        }

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_report_to_the_monitor_key_chosen_with_WithCronitorMonitorKey_on_an_added_job()
    {
        var telemetries = new FakeTelemetriesClient();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ITelemetriesClient>(telemetries);
        services.AddQuartz(q =>
        {
            q.SchedulerName = Guid.NewGuid().ToString("N");
            q.UseCronitor();
            q.AddJob<SucceedingJob>(new JobKey("backup"), job => job.WithCronitorMonitorKey("nightly-backup"));
            q.AddTrigger(trigger => trigger.ForJob("backup").StartNow());
        });

        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = await provider.GetRequiredService<ISchedulerFactory>().GetScheduler(TestContext.Current.CancellationToken);
            await scheduler.Start(TestContext.Current.CancellationToken);
            await Task.WhenAny(telemetries.Finished, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
            await scheduler.Shutdown(true, TestContext.Current.CancellationToken);
        }

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_throw_when_UseCronitor_is_called_without_configuring_Cronitor()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddQuartz(q =>
        {
            q.SchedulerName = Guid.NewGuid().ToString("N");
            q.UseCronitor();
        });

        using (var provider = services.BuildServiceProvider())
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => provider.GetRequiredService<ISchedulerFactory>().GetScheduler(TestContext.Current.CancellationToken));

            Assert.Contains("ITelemetriesClient", exception.GetBaseException().Message);
        }
    }

    private sealed class SucceedingJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }
}
