using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.Coravel;

public static class CronitorCoravelServiceCollectionExtensions
{
    /// <summary>
    /// Registers what the jobs scheduled with <c>ScheduleWithCronitor</c> need to report to Cronitor, and waits up to
    /// five seconds for reports still being sent when the application stops. Cronitor must be registered with
    /// <c>ConfigureCronitor(apiKey)</c> on the host builder.
    /// </summary>
    public static IServiceCollection AddCronitorCoravel(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton(provider => new CronitorReporter(
            TelemetriesClient(provider),
            provider.GetService<ILogger<CronitorReporter>>(),
            provider.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping));
        services.AddHostedService(provider => new CronitorShutdownService(provider.GetRequiredService<CronitorReporter>()));

        return services;
    }

    private static ITelemetriesClient TelemetriesClient(IServiceProvider provider) =>
        provider.GetService<ITelemetriesClient>()
        ?? throw new InvalidOperationException(
            "No ITelemetriesClient is registered. Register Cronitor with ConfigureCronitor(apiKey) on the host builder.");
}
