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
    }
}
