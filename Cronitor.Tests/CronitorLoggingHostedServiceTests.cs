using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

public class CronitorLoggingHostedServiceTests
{
    [Fact]
    public async Task StartAsyncShouldConfigureWithTheLoggerFactory()
    {
        var loggerFactory = new FakeLoggerFactory();
        ILoggerFactory configuredWith = null;
        var service = new CronitorLoggingHostedService(factory => configuredWith = factory, loggerFactory);

        await service.StartAsync(TestContext.Current.CancellationToken);

        Assert.Same(loggerFactory, configuredWith);
    }

    [Fact]
    public async Task StopAsyncShouldNotConfigure()
    {
        var configured = false;
        var service = new CronitorLoggingHostedService(_ => configured = true, new FakeLoggerFactory());

        await service.StopAsync(TestContext.Current.CancellationToken);

        Assert.False(configured);
    }
}
