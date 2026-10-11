using Hangfire;
using Microsoft.Extensions.Logging;
using System;

namespace Cronitor.Hangfire;

public static class CronitorGlobalConfigurationExtensions
{
    /// <summary>
    /// Reports every recurring job Hangfire runs to Cronitor with a <see cref="CronitorJobFilter"/>, using the
    /// <see cref="ITelemetriesClient"/> registered with <c>ConfigureCronitor(apiKey)</c> on the host builder.
    /// Call it from <c>services.AddHangfire((provider, configuration) => ...)</c>.
    /// </summary>
    public static IGlobalConfiguration<CronitorJobFilter> UseCronitor(this IGlobalConfiguration configuration, IServiceProvider provider)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (provider == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var telemetries = provider.GetService(typeof(ITelemetriesClient)) as ITelemetriesClient
            ?? throw new InvalidOperationException(
                "No ITelemetriesClient is registered. Register Cronitor before Hangfire with ConfigureCronitor(apiKey) on the host builder.");
        var logger = provider.GetService(typeof(ILogger<CronitorJobFilter>)) as ILogger<CronitorJobFilter>;

        return configuration.UseFilter(new CronitorJobFilter(telemetries, logger));
    }

    /// <summary>
    /// Reports every recurring job Hangfire runs to Cronitor with a <see cref="CronitorJobFilter"/>, using the
    /// static Cronitor client. Call <c>Cronitor.Configure(apiKey)</c> first.
    /// </summary>
    public static IGlobalConfiguration<CronitorJobFilter> UseCronitor(this IGlobalConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        return configuration.UseFilter(new CronitorJobFilter(global::Cronitor.Cronitor.Telemetries));
    }
}
