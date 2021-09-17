using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetryClientTests
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryClientTests()
        {
            _telemetryClient = new TelemetryClient("7512d43fa0884893b2e0533c44971fa1");
        }

        [Fact]
        public void ShouldInsertMultipleItems()
        {
            _telemetryClient.Complete("v26rUxXl");
        }
    }
}
