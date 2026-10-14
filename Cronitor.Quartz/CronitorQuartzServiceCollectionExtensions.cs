using Cronitor.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;

namespace Cronitor.Quartz;

public static class CronitorQuartzServiceCollectionExtensions
{
    /// <summary>
    /// Gives each job's Cronitor monitor the schedules and time zone of the job's triggers once the application has
    /// started, so Cronitor alerts when a job doesn't run. Requires Quartz and <c>ConfigureCronitor(apiKey)</c>.
    /// </summary>
    public static IServiceCollection AddCronitorQuartzScheduleSync(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddHostedService(provider => new CronitorScheduleSyncService(
            Schedulers(provider),
            new ScheduleSync(Monitors(provider), provider.GetService<ILogger<ScheduleSync>>()),
            provider.GetRequiredService<IHostApplicationLifetime>(),
            provider.GetService<ILogger<CronitorScheduleSyncService>>()));

        return services;
    }

    private static ISchedulerFactory Schedulers(IServiceProvider provider) =>
        provider.GetService<ISchedulerFactory>()
        ?? throw new InvalidOperationException("Quartz is not registered. Call AddQuartz(...) before AddCronitorQuartzScheduleSync().");

    private static IMonitorsClient Monitors(IServiceProvider provider) =>
        provider.GetService<IMonitorsClient>()
        ?? throw new InvalidOperationException("No IMonitorsClient is registered. Register Cronitor with ConfigureCronitor(apiKey) on the host builder.");
}
