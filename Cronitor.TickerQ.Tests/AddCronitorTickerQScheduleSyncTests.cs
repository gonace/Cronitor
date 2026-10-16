using Cronitor.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TickerQ.DependencyInjection;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Interfaces;
using TickerQ.Utilities.Interfaces.Managers;
using Xunit;

namespace Cronitor.TickerQ.Tests;

[Collection(TickerQHostCollection.Name)]
public class AddCronitorTickerQScheduleSyncTests(TickerQHost host)
{
    [Fact]
    public async Task Should_sync_the_schedules_of_the_cron_tickers_in_TickerQ_s_store_once_the_application_has_started()
    {
        var cronTickers = host.Services.GetRequiredService<ICronTickerManager<CronTickerEntity>>();
        var cronTicker = new CronTickerEntity { Function = nameof(Jobs.SucceedingJob), Expression = "0 0 3 29 2 *" };
        Assert.True((await cronTickers.AddAsync(cronTicker, TestContext.Current.CancellationToken)).IsSucceeded);
        var monitors = new FakeMonitorsClient();
        var lifetime = new TestLifetime();
        var store = CronTickerStore.Of(host.Services, typeof(ITickerPersistenceProvider<TimeTickerEntity, CronTickerEntity>));
        var service = new CronitorScheduleSyncService(store, TimeZoneInfo.Utc, new ScheduleSync(monitors), lifetime);

        try
        {
            await service.StartAsync(TestContext.Current.CancellationToken);
            lifetime.NotifyStarted();
            await service.Synced;
        }
        finally
        {
            await cronTickers.DeleteAsync(cronTicker.Id, TestContext.Current.CancellationToken);
        }

        Assert.Contains("""{"key":"SucceedingJob","schedules":["0 3 29 2 *"],"timezone":"UTC","type":"job"}""", Assert.Single(monitors.Updates));
    }

    [Fact]
    public void Should_register_the_sync_as_a_hosted_service()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostApplicationLifetime>(new TestLifetime());
        services.AddSingleton<IMonitorsClient>(new FakeMonitorsClient());
        services.AddTickerQ();

        services.AddCronitorTickerQScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            Assert.IsType<CronitorScheduleSyncService>(LastHostedService(services, provider));
        }
    }

    [Fact]
    public void Should_throw_when_AddCronitorTickerQScheduleSync_is_called_before_AddTickerQ()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddCronitorTickerQScheduleSync());

        Assert.Contains("after AddTickerQ()", exception.Message);
    }

    [Fact]
    public void Should_throw_when_Cronitor_is_not_configured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostApplicationLifetime>(new TestLifetime());
        services.AddTickerQ();
        services.AddCronitorTickerQScheduleSync();

        using (var provider = services.BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => LastHostedService(services, provider));

            Assert.Contains("ConfigureCronitor", exception.Message);
        }
    }

    private static object LastHostedService(IServiceCollection services, IServiceProvider provider) =>
        services.Last(service => service.ServiceType == typeof(IHostedService)).ImplementationFactory(provider);
}