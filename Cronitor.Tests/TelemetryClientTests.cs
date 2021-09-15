using Xunit;

namespace Cronitor.Tests
{
    public class TelemetryClientTests
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryClientTests()
        {
            _telemetryClient = new TelemetryClient("");
        }

        [Fact]
        public void ShouldInsertMultipleItems()
        {
            _telemetryClient.Run("");
        }
    }
}
