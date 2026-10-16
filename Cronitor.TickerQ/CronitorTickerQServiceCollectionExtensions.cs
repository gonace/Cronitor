using Cronitor.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TickerQ.Utilities;
using TickerQ.Utilities.Interfaces;

namespace Cronitor.TickerQ;

public static class CronitorTickerQServiceCollectionExtensions
{
    /// <summary>
    /// Reports every cron job TickerQ runs to Cronitor, using the job's function name as the monitor key, and waits
    /// up to five seconds for reports still being sent when the application stops. Call it after <c>AddTickerQ()</c>,
    /// since it wraps the job handler TickerQ registers. Cronitor must be registered with <c>ConfigureCronitor(apiKey)</c>
    /// on the host builder.
    /// </summary>
    public static IServiceCollection AddCronitorTickerQ(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var tickerQHandler = services.LastOrDefault(service => service.ServiceType == typeof(ITickerExecutionTaskHandler))
            ?? throw new InvalidOperationException("TickerQ is not registered. Call AddCronitorTickerQ() after AddTickerQ().");
        var tickerQHandlerType = tickerQHandler.ImplementationType
            ?? throw new InvalidOperationException("TickerQ's job handler is registered in a way AddCronitorTickerQ() can't wrap. Call AddCronitorTickerQ() once, after AddTickerQ().");

        services.Remove(tickerQHandler);
        services.AddSingleton(tickerQHandlerType);
        services.AddSingleton(provider => new CronitorExecutionTaskHandler(
            (ITickerExecutionTaskHandler)provider.GetRequiredService(tickerQHandlerType),
            TelemetriesClient(provider),
            provider.GetService<ILogger<CronitorExecutionTaskHandler>>()));
        services.AddSingleton<ITickerExecutionTaskHandler>(provider => provider.GetRequiredService<CronitorExecutionTaskHandler>());
        services.AddHostedService(provider => new CronitorShutdownService(provider.GetRequiredService<CronitorExecutionTaskHandler>()));

        return services;
    }

    /// <summary>
    /// Gives each cron job's Cronitor monitor the schedules of its cron tickers, in TickerQ's time zone, once the
    /// application has started, so Cronitor alerts when a job doesn't run. Call it after <c>AddTickerQ()</c>. Cronitor
    /// must be registered with <c>ConfigureCronitor(apiKey)</c> on the host builder.
    /// </summary>
    public static IServiceCollection AddCronitorTickerQScheduleSync(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var persistenceProviderType = services.LastOrDefault(service => IsPersistenceProvider(service.ServiceType))?.ServiceType
            ?? throw new InvalidOperationException("TickerQ is not registered. Call AddCronitorTickerQScheduleSync() after AddTickerQ().");

        services.AddHostedService(provider => new CronitorScheduleSyncService(
            CronTickerStore.Of(provider, persistenceProviderType),
            provider.GetRequiredService<SchedulerOptionsBuilder>().SchedulerTimeZone,
            new ScheduleSync(MonitorsClient(provider), provider.GetService<ILogger<ScheduleSync>>()),
            provider.GetRequiredService<IHostApplicationLifetime>(),
            provider.GetService<ILogger<CronitorScheduleSyncService>>()));

        return services;
    }

    private static bool IsPersistenceProvider(Type serviceType) =>
        serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(ITickerPersistenceProvider<,>);

    private static IMonitorsClient MonitorsClient(IServiceProvider provider) =>
        provider.GetService<IMonitorsClient>()
        ?? throw new InvalidOperationException("No IMonitorsClient is registered. Register Cronitor with ConfigureCronitor(apiKey) on the host builder.");

    private static ITelemetriesClient TelemetriesClient(IServiceProvider provider) =>
        provider.GetService<ITelemetriesClient>()
        ?? throw new InvalidOperationException(
            "No ITelemetriesClient is registered. Register Cronitor with ConfigureCronitor(apiKey) on the host builder.");
}
