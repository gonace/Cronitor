using Cronitor;
using Hangfire.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Hangfire.Extensions.Cronitor.Tests;

/// <summary>
/// Registers the filter through Hangfire's global configuration, the way an application would.
/// </summary>
[Collection(GlobalJobFiltersCollection.Name)]
public class UseCronitorTests : IDisposable
{
    /// <summary>
    /// <c>UseCronitor</c> adds to Hangfire's global filter list, so the next test must not inherit the filter.
    /// </summary>
    public void Dispose()
    {
        foreach (var filter in GlobalJobFilters.Filters.Where(filter => filter.Instance is CronitorJobFilter).ToList())
        {
            GlobalJobFilters.Filters.Remove(filter.Instance);
        }
    }

    [Fact]
    public async Task Should_report_recurring_jobs_when_UseCronitor_is_called_with_the_service_provider()
    {
        var telemetries = new FakeTelemetriesClient();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ITelemetriesClient>(telemetries);

        using (var provider = services.BuildServiceProvider())
        {
            GlobalConfiguration.Configuration.UseCronitor(provider);
            await TestServer.RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), JobFilterProviders.Providers);
        }

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public void Should_throw_when_UseCronitor_is_called_with_a_service_provider_without_Cronitor()
    {
        using (var provider = new ServiceCollection().BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => GlobalConfiguration.Configuration.UseCronitor(provider));

            Assert.Contains("ConfigureCronitor", exception.Message);
        }
    }

    [Fact]
    public void Should_register_the_filter_when_UseCronitor_is_called_after_configuring_Cronitor()
    {
        global::Cronitor.Cronitor.Configure("apiKey");

        GlobalConfiguration.Configuration.UseCronitor();

        Assert.Single(GlobalJobFilters.Filters, filter => filter.Instance is CronitorJobFilter);
    }
}
