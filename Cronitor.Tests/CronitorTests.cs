using Cronitor.Exceptions;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests
{
    public class CronitorTests : BaseTest
    {
        [Fact]
        public void ShouldConfigureCronitor()
        {
            Cronitor.Configure(ApiKey);

            Assert.NotNull(Cronitor.Issues);
            Assert.NotNull(Cronitor.Monitors);
            Assert.NotNull(Cronitor.Notifications);
            Assert.NotNull(Cronitor.Telemetries);
        }

        [Fact(Explicit = true)]
        public void ShouldThrowExceptionIfNotConfigured()
        {
            Assert.Throws<NotConfiguredException>(() => Cronitor.Issues);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Monitors);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Notifications);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Telemetries);
        }
    }
}
