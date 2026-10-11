using Cronitor.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Cronitor.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureCronitor(this IHostBuilder builder, string apiKey)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ConfigureServices((context, services) =>
        {
            services.AddTransient<IIssuesClient>((_) => new IssuesClient(apiKey));
            services.AddTransient<IMonitorsClient>((_) => new MonitorsClient(apiKey));
            services.AddTransient<INotificationsClient>((_) => new NotificationsClient(apiKey));
            services.AddTransient<ITelemetriesClient>(provider => new TelemetriesClient(apiKey, LoggerFactory(provider)));
        });

        return builder;
    }

    public static IHostBuilder ConfigureCronitor(this IHostBuilder builder, Func<HostBuilderContext, string> optionsBuilder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ConfigureServices((context, services) =>
        {
            var apiKey = optionsBuilder(context);
            services.AddTransient<IIssuesClient>((_) => new IssuesClient(apiKey));
            services.AddTransient<IMonitorsClient>((_) => new MonitorsClient(apiKey));
            services.AddTransient<INotificationsClient>((_) => new NotificationsClient(apiKey));
            services.AddTransient<ITelemetriesClient>(provider => new TelemetriesClient(apiKey, LoggerFactory(provider)));
        });

        return builder;
    }

    public static IHostBuilder UseCronitor(this IHostBuilder builder, string apiKey)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        UseCronitor(apiKey);
        builder.ConfigureServices((_, services) => AddLogging(services, apiKey));

        return builder;
    }

    public static IHostBuilder UseCronitor(this IHostBuilder builder, Func<HostBuilderContext, string> optionsBuilder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ConfigureServices((context, services) =>
        {
            var apiKey = optionsBuilder(context);
            UseCronitor(apiKey);
            AddLogging(services, apiKey);
        });

        return builder;
    }

    private static void UseCronitor(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

        Cronitor.Configure(apiKey);
    }

    /// <summary>
    /// Reconfigures the static Cronitor class with the host's logging when the host starts,
    /// since its <see cref="ILoggerFactory"/> doesn't exist yet when UseCronitor runs.
    /// </summary>
    private static void AddLogging(IServiceCollection services, string apiKey)
    {
        services.AddHostedService(provider => new CronitorLoggingHostedService(
            loggerFactory => Cronitor.Configure(apiKey, loggerFactory),
            LoggerFactory(provider)));
    }

    private static ILoggerFactory LoggerFactory(IServiceProvider provider)
    {
        return provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
    }
}
