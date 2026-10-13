using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Runtime.CompilerServices;

namespace Cronitor.Quartz;

public static class CronitorQuartzConfiguratorExtensions
{
    /// <summary>
    /// Reports every job Quartz runs to Cronitor with a <see cref="CronitorJobListener"/>, and waits for reports
    /// still being sent when the scheduler shuts down with a <see cref="CronitorSchedulerListener"/>.
    /// Cronitor must be registered first with <c>ConfigureCronitor(apiKey)</c> on the host builder.
    /// </summary>
    public static IQuartzBuilder UseCronitor(this IQuartzBuilder quartz)
    {
        if (quartz == null)
        {
            throw new ArgumentNullException(nameof(quartz));
        }

        // Both listeners must share one job listener, since the scheduler listener waits for its reports.
        var jobListeners = new ConditionalWeakTable<IServiceProvider, CronitorJobListener>();
        CronitorJobListener JobListener(IServiceProvider provider) => jobListeners.GetValue(provider, CreateJobListener);

        // Quartz 4 has no default for the matchers; this one matches every job, like Quartz 3's default.
        quartz.AddJobListener(JobListener, [Matchers.AllJobs()]);
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
