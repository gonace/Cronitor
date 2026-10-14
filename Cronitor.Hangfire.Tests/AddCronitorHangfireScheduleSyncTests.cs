using Cronitor.Scheduling;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Hangfire.Tests;

public class AddCronitorHangfireScheduleSyncTests
{
    [Fact]
    public async Task Should_sync_the_schedules_of_the_recurring_jobs_in_storage_once_the_application_has_started()
    {
        var storage = new InMemoryStorage();
        new RecurringJobManager(storage).AddOrUpdate("nightly-backup", () => TestJobs.Succeed(), "0 3 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
        var monitors = new FakeMonitorsClient();
        var lifetime = new TestLifetime();
        var service = new CronitorScheduleSyncService(storage, new ScheduleSync(monitors), lifetime);

        await service.StartAsync(TestContext.Current.CancellationToken);
        lifetime.NotifyStarted();
        await service.Synced;

        Assert.Equal("""{"monitors":[{"key":"nightly-backup","schedules":["0 3 * * *"],"timezone":"UTC","type":"job"}]}""", Assert.Single(monitors.Updates));
    }

    [Fact]
    public async Task Should_not_sync_before_the_application_has_started()
    {
        var storage = new InMemoryStorage();
        new RecurringJobManager(storage).AddOrUpdate("nightly-backup", () => TestJobs.Succeed(), "0 3 * * *");
        var monitors = new FakeMonitorsClient();
        var service = new CronitorScheduleSyncService(storage, new ScheduleSync(monitors), new TestLifetime());

        await service.StartAsync(TestContext.Current.CancellationToken);
        await service.Synced;

        Assert.Empty(monitors.Updates);
    }

    [Fact]
    public async Task Should_not_hold_up_the_application_while_it_syncs()
    {
        var storage = new InMemoryStorage();
        new RecurringJobManager(storage).AddOrUpdate("nightly-backup", () => TestJobs.Succeed(), "0 3 * * *");
        var monitors = new FakeMonitorsClient { UpdateGate = new TaskCompletionSource<bool>().Task };
        var lifetime = new TestLifetime();
        var service = new CronitorScheduleSyncService(storage, new ScheduleSync(monitors), lifetime);
        await service.StartAsync(TestContext.Current.CancellationToken);

        lifetime.NotifyStarted();

        Assert.False(service.Synced.IsCompleted);
    }

    [Fact]
    public void Should_register_the_sync_as_a_hosted_service()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostApplicationLifetime>(new TestLifetime());
        services.AddSingleton<JobStorage>(new InMemoryStorage());
        services.AddSingleton<IMonitorsClient>(new FakeMonitorsClient());

        services.AddCronitorHangfireScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            Assert.Single(provider.GetServices<IHostedService>().OfType<CronitorScheduleSyncService>());
        }
    }

    [Fact]
    public void Should_throw_when_Hangfire_is_not_registered()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMonitorsClient>(new FakeMonitorsClient());
        services.AddCronitorHangfireScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetServices<IHostedService>().ToList());

            Assert.Contains("AddHangfire", exception.Message);
        }
    }

    [Fact]
    public void Should_throw_when_Cronitor_is_not_configured()
    {
        var services = new ServiceCollection();
        services.AddSingleton<JobStorage>(new InMemoryStorage());
        services.AddCronitorHangfireScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetServices<IHostedService>().ToList());

            Assert.Contains("ConfigureCronitor", exception.Message);
        }
    }

    private sealed class TestLifetime : IHostApplicationLifetime
    {
        private readonly CancellationTokenSource _started = new CancellationTokenSource();

        public CancellationToken ApplicationStarted => _started.Token;

        public CancellationToken ApplicationStopping => CancellationToken.None;

        public CancellationToken ApplicationStopped => CancellationToken.None;

        public void NotifyStarted() => _started.Cancel();

        public void StopApplication()
        {
        }
    }
}
