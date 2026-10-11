using Cronitor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace Quartz;

public static class CronitorQuartzConfiguratorExtensions
{
    /// <summary>
    /// Reports every job Quartz runs to Cronitor with a <see cref="CronitorJobListener"/>, and waits for reports
    /// still being sent when the scheduler shuts down with a <see cref="CronitorSchedulerListener"/>.
    /// Cronitor must be registered first with <c>ConfigureCronitor(apiKey)</c> on the host builder.
    /// </summary>
    public static IServiceCollectionQuartzConfigurator UseCronitor(this IServiceCollectionQuartzConfigurator quartz)
    {
        if (quartz == null)
        {
            throw new ArgumentNullException(nameof(quartz));
        }

        // Both listeners must share one job listener, since the scheduler listener waits for its reports.
        var jobListeners = new ConditionalWeakTable<IServiceProvider, CronitorJobListener>();
        CronitorJobListener JobListener(IServiceProvider provider) => jobListeners.GetValue(provider, CreateJobListener);

        quartz.AddJobListener(JobListener);
        quartz.AddSchedulerListener(provider => new CronitorSchedulerListener(JobListener(provider)));

        return quartz;
    }

    private static CronitorJobListener CreateJobListener(IServiceProvider provider) =>
        new CronitorJobListener(TelemetriesClient(provider), provider.GetService<ILogger<CronitorJobListener>>());

    private static ITelemetriesClient TelemetriesClient(IServiceProvider provider) =>
        provider.GetService<ITelemetriesClient>()
        ?? throw new InvalidOperationException(
            "No ITelemetriesClient is registered. Register Cronitor before Quartz with ConfigureCronitor(apiKey) on the host builder.");
}
