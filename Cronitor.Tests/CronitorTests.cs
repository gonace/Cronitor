using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests
{
    public class CronitorTests : BaseTest
    {
        [Fact]
        public void ShouldInstanciateCronitor()
        {
            // Run
            Cronitor.Configure(ApiKey);

            // Assert
            Assert.NotNull(Cronitor.Monitor);
            Assert.NotNull(Cronitor.Notification);
            Assert.NotNull(Cronitor.Telemetry);
        }
    }
}
