using Cronitor.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Cronitor.Extensions
{
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
                services.AddTransient<ITelemetriesClient>((_) => new TelemetriesClient(apiKey));
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
                services.AddTransient<ITelemetriesClient>((_) => new TelemetriesClient(apiKey));
            });

            return builder;
        }


        public static IHostBuilder UseCronitor(this IHostBuilder builder, string apiKey)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            UseCronitor(apiKey);

            return builder;
        }

        public static IHostBuilder UseCronitor(this IHostBuilder builder, Func<HostBuilderContext, string> optionsBuilder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((context, services) =>
            {
                var apiKey = optionsBuilder(context);
                UseCronitor(apiKey);
            });

            return builder;
        }

        private static void UseCronitor(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

            Cronitor.Configure(apiKey);
        }
    }
}
