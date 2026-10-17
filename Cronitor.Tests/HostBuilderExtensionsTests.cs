using Cronitor.Extensions;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;
using Host = Microsoft.Extensions.Hosting.Host;

namespace Cronitor.Tests;

[Collection(StaticCronitorCollection.Name)]
public class HostBuilderExtensionsTests : BaseTest
{
    [Fact]
    public void Should_register_all_services_when_ConfigureCronitor_gets_an_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var result = hostBuilder.ConfigureCronitor(ApiKey);

        Assert.NotNull(result);
        var host = hostBuilder.Build();

        var issuesClient = host.Services.GetService<IIssuesClient>();
        var monitorsClient = host.Services.GetService<IMonitorsClient>();
        var notificationsClient = host.Services.GetService<INotificationsClient>();
        var telemetriesClient = host.Services.GetService<ITelemetriesClient>();

        Assert.NotNull(issuesClient);
        Assert.NotNull(monitorsClient);
        Assert.NotNull(notificationsClient);
        Assert.NotNull(telemetriesClient);
    }

    [Fact]
    public void Should_register_services_as_transient_when_ConfigureCronitor_gets_an_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        hostBuilder.ConfigureCronitor(ApiKey);
        var host = hostBuilder.Build();

        var issuesClient1 = host.Services.GetService<IIssuesClient>();
        var issuesClient2 = host.Services.GetService<IIssuesClient>();

        Assert.NotNull(issuesClient1);
        Assert.NotNull(issuesClient2);
        Assert.NotSame(issuesClient1, issuesClient2);
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_ConfigureCronitor_gets_a_null_builder()
    {
        IHostBuilder builder = null;

        Assert.Throws<ArgumentNullException>(() => builder.ConfigureCronitor(ApiKey));
    }

    [Fact]
    public void Should_register_all_services_when_ConfigureCronitor_gets_an_options_builder()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var result = hostBuilder.ConfigureCronitor(context => ApiKey);

        Assert.NotNull(result);
        var host = hostBuilder.Build();

        var issuesClient = host.Services.GetService<IIssuesClient>();
        var monitorsClient = host.Services.GetService<IMonitorsClient>();
        var notificationsClient = host.Services.GetService<INotificationsClient>();
        var telemetriesClient = host.Services.GetService<ITelemetriesClient>();

        Assert.NotNull(issuesClient);
        Assert.NotNull(monitorsClient);
        Assert.NotNull(notificationsClient);
        Assert.NotNull(telemetriesClient);
    }

    [Fact]
    public void Should_invoke_the_context_function_when_ConfigureCronitor_gets_an_options_builder()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        var functionCalled = false;

        hostBuilder.ConfigureCronitor(context =>
        {
            functionCalled = true;
            Assert.NotNull(context);
            return ApiKey;
        });

        var host = hostBuilder.Build();
        var client = host.Services.GetService<IIssuesClient>();

        Assert.True(functionCalled);
        Assert.NotNull(client);
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_ConfigureCronitor_gets_an_options_builder_and_a_null_builder()
    {
        IHostBuilder builder = null;

        Assert.Throws<ArgumentNullException>(() =>
            builder.ConfigureCronitor(context => ApiKey));
    }

    [Fact]
    public void Should_return_the_builder_when_UseCronitor_gets_an_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var result = hostBuilder.UseCronitor(ApiKey);

        Assert.NotNull(result);
        Assert.Same(hostBuilder, result);
    }

    [Fact]
    public void Should_configure_CronitorClient_when_UseCronitor_gets_an_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.UseCronitor(ApiKey);

        // Verify CronitorClient is configured by accessing its properties
        Assert.NotNull(CronitorClient.Issues);
        Assert.NotNull(CronitorClient.Monitors);
        Assert.NotNull(CronitorClient.Notifications);
        Assert.NotNull(CronitorClient.Telemetries);
    }

    [Fact]
    public void Should_register_the_logging_hosted_service_when_UseCronitor_gets_an_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var host = hostBuilder.UseCronitor(ApiKey).Build();

        Assert.Contains(host.Services.GetServices<IHostedService>(), service => service is CronitorLoggingHostedService);
    }

    [Fact]
    public void Should_register_the_logging_hosted_service_when_UseCronitor_gets_an_options_builder()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var host = hostBuilder.UseCronitor(context => ApiKey).Build();

        Assert.Contains(host.Services.GetServices<IHostedService>(), service => service is CronitorLoggingHostedService);
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_UseCronitor_gets_a_null_builder()
    {
        IHostBuilder builder = null;

        Assert.Throws<ArgumentNullException>(() => builder.UseCronitor(ApiKey));
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_UseCronitor_gets_a_null_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        Assert.Throws<ArgumentNullException>(() => hostBuilder.UseCronitor((string)null));
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_UseCronitor_gets_an_empty_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        Assert.Throws<ArgumentNullException>(() => hostBuilder.UseCronitor(string.Empty));
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_UseCronitor_gets_a_whitespace_api_key()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        Assert.Throws<ArgumentNullException>(() => hostBuilder.UseCronitor("   "));
    }

    [Fact]
    public void Should_configure_CronitorClient_when_UseCronitor_gets_an_options_builder()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.UseCronitor(context => ApiKey);
        var host = hostBuilder.Build();

        // Verify CronitorClient is configured
        Assert.NotNull(host);
        Assert.NotNull(CronitorClient.Issues);
        Assert.NotNull(CronitorClient.Monitors);
        Assert.NotNull(CronitorClient.Notifications);
        Assert.NotNull(CronitorClient.Telemetries);
    }

    [Fact]
    public void Should_throw_ArgumentNullException_when_UseCronitor_gets_an_options_builder_and_a_null_builder()
    {
        IHostBuilder builder = null;

        Assert.Throws<ArgumentNullException>(() =>
            builder.UseCronitor(context => ApiKey));
    }
}
