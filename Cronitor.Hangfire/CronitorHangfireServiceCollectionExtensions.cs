using Cronitor.Scheduling;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.Hangfire;

public static class CronitorHangfireServiceCollectionExtensions
{
    /// <summary>
    /// Gives each recurring job's Cronitor monitor the job's schedule and time zone once the application has started,
    /// so Cronitor alerts when a job doesn't run. Requires Hangfire and <c>ConfigureCronitor(apiKey)</c>.
    /// </summary>
    public static IServiceCollection AddCronitorHangfireScheduleSync(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddHostedService(provider => new CronitorScheduleSyncService(
            Storage(provider),
            new ScheduleSync(Monitors(provider), provider.GetService<ILogger<ScheduleSync>>()),
            provider.GetRequiredService<IHostApplicationLifetime>(),
            provider.GetService<ILogger<CronitorScheduleSyncService>>()));

        return services;
    }

    private static JobStorage Storage(IServiceProvider provider) =>
        provider.GetService<JobStorage>()
        ?? throw new InvalidOperationException("Hangfire is not registered. Call AddHangfire(...) before AddCronitorHangfireScheduleSync().");

    private static IMonitorsClient Monitors(IServiceProvider provider) =>
        provider.GetService<IMonitorsClient>()
        ?? throw new InvalidOperationException("No IMonitorsClient is registered. Register Cronitor with ConfigureCronitor(apiKey) on the host builder.");
}
