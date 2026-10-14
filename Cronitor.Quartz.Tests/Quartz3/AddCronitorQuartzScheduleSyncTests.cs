using Cronitor.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Quartz.Tests;

[Collection(StaticQuartzCollection.Name)]
public class AddCronitorQuartzScheduleSyncTests : IDisposable
{
    public void Dispose() => LogProvider.SetCurrentLogProvider(null);

    [Fact]
    public async Task Should_sync_the_schedules_of_the_scheduled_jobs_once_the_application_has_started()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddQuartz(q => q.SchedulerName = Guid.NewGuid().ToString("N"));
        var monitors = new FakeMonitorsClient();
        var lifetime = new TestLifetime();

        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = await provider.GetRequiredService<ISchedulerFactory>().GetScheduler(TestContext.Current.CancellationToken);
            var job = JobBuilder.Create<NoopJob>().WithIdentity("quartz3-schedule-sync").Build();
            await scheduler.ScheduleJob(job, TriggerBuilder.Create().WithCronSchedule("0 0 3 ? * 2", schedule => schedule.InTimeZone(TimeZoneInfo.Utc)).Build(), TestContext.Current.CancellationToken);
            var service = new CronitorScheduleSyncService(provider.GetRequiredService<ISchedulerFactory>(), new ScheduleSync(monitors), lifetime);

            await service.StartAsync(TestContext.Current.CancellationToken);
            lifetime.NotifyStarted();
            await service.Synced;
            await scheduler.Shutdown(TestContext.Current.CancellationToken);
        }

        Assert.Contains("""{"key":"quartz3-schedule-sync","schedules":["0 3 * * 1"],"timezone":"UTC","type":"job"}""", Assert.Single(monitors.Updates));
    }

    [Fact]
    public void Should_register_the_sync_as_a_hosted_service()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostApplicationLifetime>(new TestLifetime());
        services.AddQuartz(q => q.SchedulerName = Guid.NewGuid().ToString("N"));
        services.AddSingleton<IMonitorsClient>(new FakeMonitorsClient());

        services.AddCronitorQuartzScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            Assert.Single(provider.GetServices<IHostedService>().OfType<CronitorScheduleSyncService>());
        }
    }

    [Fact]
    public void Should_throw_when_Quartz_is_not_registered()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMonitorsClient>(new FakeMonitorsClient());
        services.AddCronitorQuartzScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetServices<IHostedService>().ToList());

            Assert.Contains("AddQuartz", exception.Message);
        }
    }
}
