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
            // Run
            Cronitor.Configure(ApiKey);

            // Assert
            Assert.True(Cronitor.Configured);
            Assert.NotNull(Cronitor.Issues);
            Assert.NotNull(Cronitor.Monitors);
            Assert.NotNull(Cronitor.Notifications);
            Assert.NotNull(Cronitor.Telemetries);
        }

        [Fact]
        public void ShouldThrowExceptionIfNotConfigured()
        {
            // Assert
            Assert.False(Cronitor.Configured);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Issues);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Monitors);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Notifications);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Telemetries);
        }
    }
}
