using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

public class CronitorLoggingHostedServiceTests
{
    [Fact]
    public async Task Should_configure_with_the_logger_factory_in_StartAsync()
    {
        var loggerFactory = new FakeLoggerFactory();
        ILoggerFactory configuredWith = null;
        var service = new CronitorLoggingHostedService(factory => configuredWith = factory, loggerFactory);

        await service.StartAsync(TestContext.Current.CancellationToken);

        Assert.Same(loggerFactory, configuredWith);
    }

    [Fact]
    public async Task Should_not_configure_in_StopAsync()
    {
        var configured = false;
        var service = new CronitorLoggingHostedService(_ => configured = true, new FakeLoggerFactory());

        await service.StopAsync(TestContext.Current.CancellationToken);

        Assert.False(configured);
    }
}
