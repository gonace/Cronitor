using Cronitor;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Quartz;

public static class CronitorQuartzConfiguratorExtensions
{
    /// <summary>
    /// Reports every job Quartz runs to Cronitor with a <see cref="CronitorJobListener"/>.
    /// Cronitor must be registered first with <c>ConfigureCronitor(apiKey)</c> on the host builder.
    /// </summary>
    public static IServiceCollectionQuartzConfigurator UseCronitor(this IServiceCollectionQuartzConfigurator quartz)
    {
        if (quartz == null)
        {
            throw new ArgumentNullException(nameof(quartz));
        }

        quartz.AddJobListener(provider => new CronitorJobListener(TelemetriesClient(provider)));

        return quartz;
    }

    private static ITelemetriesClient TelemetriesClient(IServiceProvider provider) =>
        provider.GetService<ITelemetriesClient>()
        ?? throw new InvalidOperationException(
            "No ITelemetriesClient is registered. Register Cronitor before Quartz with ConfigureCronitor(apiKey) on the host builder.");
}
