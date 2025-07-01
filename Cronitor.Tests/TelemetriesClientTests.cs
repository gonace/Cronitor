using Cronitor.Clients;
using Cronitor.Commands;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class TelemetriesClientTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClientTests()
        {
            _httpClient = new Mock<HttpClient>();
            _telemetriesClient = new TelemetriesClient(ApiKey, _httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteWhenUtilizingUsingBlock()
        {
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
             _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

             _telemetriesClient.Run(MonitorKey);

             _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                 c.ApiKey == ApiKey &&
                 c.MonitorKey == MonitorKey &&
                 c.Endpoint == "run")), Times.Once);
             _httpClient.VerifyNoOtherCalls();
        }
    }
}