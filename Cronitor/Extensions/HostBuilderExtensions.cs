using System;
using Microsoft.Extensions.Hosting;

namespace Cronitor.Extensions
{
    public static class HostBuilderExtensions
    {
        [Obsolete("This method has no more usages and will be removed in a future version, please use ConfigureCronitor(string apiKey) instead.")]
        public static IHostBuilder UseCronitor(this IHostBuilder builder, string apiKey) =>
            builder.ConfigureCronitor(apiKey);

        [Obsolete("This method has no more usages and will be removed in a future version, please use ConfigureCronitor(string apiKey) instead.")]
        public static IHostBuilder UseCronitor(this IHostBuilder builder,
            Func<HostBuilderContext, string> options) =>
            builder.ConfigureCronitor(options);


        public static IHostBuilder ConfigureCronitor(this IHostBuilder builder, string apiKey)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            ConfigureCronitor(apiKey);

            return builder;
        }

        public static IHostBuilder ConfigureCronitor(this IHostBuilder builder, Func<HostBuilderContext, string> options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((context, services) =>
            {
                var apiKey = options(context);
                ConfigureCronitor(apiKey);
            });

            return builder;
        }

        private static void ConfigureCronitor(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

            Cronitor.Configure(apiKey);
        }
    }
}
