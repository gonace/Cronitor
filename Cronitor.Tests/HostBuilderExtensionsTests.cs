using Cronitor.Clients;
using Cronitor.Extensions;
using Cronitor.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;
using Host = Microsoft.Extensions.Hosting.Host;

namespace Cronitor.Tests
{
    public class HostBuilderExtensionsTests : BaseTest
    {
        [Fact]
        public void ConfigureCronitor_WithApiKey_ShouldRegisterAllServices()
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
        public void ConfigureCronitor_WithApiKey_ShouldRegisterServicesAsTransient()
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
        public void ConfigureCronitor_WithNullBuilder_ShouldThrowArgumentNullException()
        {
            IHostBuilder builder = null;

            Assert.Throws<ArgumentNullException>(() => builder.ConfigureCronitor(ApiKey));
        }

        [Fact]
        public void ConfigureCronitor_WithOptionsBuilder_ShouldRegisterAllServices()
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
        public void ConfigureCronitor_WithOptionsBuilder_ShouldInvokeContextFunction()
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
        public void ConfigureCronitor_WithOptionsBuilderAndNullBuilder_ShouldThrowArgumentNullException()
        {
            IHostBuilder builder = null;

            Assert.Throws<ArgumentNullException>(() =>
                builder.ConfigureCronitor(context => ApiKey));
        }

        [Fact]
        public void UseCronitor_WithApiKey_ShouldReturnBuilder()
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            var result = hostBuilder.UseCronitor(ApiKey);

            Assert.NotNull(result);
            Assert.Same(hostBuilder, result);
        }

        [Fact]
        public void UseCronitor_WithApiKey_ShouldConfigureStaticCronitor()
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.UseCronitor(ApiKey);

            // Verify static Cronitor is configured by accessing its properties
            Assert.NotNull(Cronitor.Issues);
            Assert.NotNull(Cronitor.Monitors);
            Assert.NotNull(Cronitor.Notifications);
            Assert.NotNull(Cronitor.Telemetries);
        }

        [Fact]
        public void UseCronitor_WithNullBuilder_ShouldThrowArgumentNullException()
        {
            IHostBuilder builder = null;

            Assert.Throws<ArgumentNullException>(() => builder.UseCronitor(ApiKey));
        }

        [Fact]
        public void UseCronitor_WithNullApiKey_ShouldThrowArgumentNullException()
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            Assert.Throws<ArgumentNullException>(() => hostBuilder.UseCronitor((string)null));
        }

        [Fact]
        public void UseCronitor_WithEmptyApiKey_ShouldThrowArgumentNullException()
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            Assert.Throws<ArgumentNullException>(() => hostBuilder.UseCronitor(string.Empty));
        }

        [Fact]
        public void UseCronitor_WithWhitespaceApiKey_ShouldThrowArgumentNullException()
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            Assert.Throws<ArgumentNullException>(() => hostBuilder.UseCronitor("   "));
        }

        [Fact]
        public void UseCronitor_WithOptionsBuilder_ShouldConfigureStaticCronitor()
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.UseCronitor(context => ApiKey);
            var host = hostBuilder.Build();

            // Verify static Cronitor is configured
            Assert.NotNull(host);
            Assert.NotNull(Cronitor.Issues);
            Assert.NotNull(Cronitor.Monitors);
            Assert.NotNull(Cronitor.Notifications);
            Assert.NotNull(Cronitor.Telemetries);
        }

        [Fact]
        public void UseCronitor_WithOptionsBuilderAndNullBuilder_ShouldThrowArgumentNullException()
        {
            IHostBuilder builder = null;

            Assert.Throws<ArgumentNullException>(() =>
                builder.UseCronitor(context => ApiKey));
        }
    }
}
