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
            Assert.NotNull(Cronitor.Monitor);
            Assert.NotNull(Cronitor.Notification);
            Assert.NotNull(Cronitor.Telemetries);
        }

        [Fact]
        public void ShouldThrowExceptionIfNotConfigured()
        {
            // Assert
            Assert.False(Cronitor.IsConfigured);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Monitor);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Notification);
            Assert.Throws<NotConfiguredException>(() => Cronitor.Telemetries);
        }
    }
}
