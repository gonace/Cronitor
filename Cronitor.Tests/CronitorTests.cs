using Cronitor.Commands;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    [Collection(StaticCronitorCollection.Name)]
    public class CronitorTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;

        public CronitorTests()
        {
            _httpClient = new Mock<HttpClient>();
            _httpClient.Setup(x => x.SendAsync(It.IsAny<Command>())).Returns(Task.CompletedTask);
        }

        [Fact]
        public void ShouldConfigureCronitor()
        {
            Cronitor.Configure(ApiKey);

            Assert.NotNull(Cronitor.Issues);
            Assert.NotNull(Cronitor.Monitors);
            Assert.NotNull(Cronitor.Notifications);
            Assert.NotNull(Cronitor.Telemetries);
        }

        [Fact]
        public void ShouldExecuteRunMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            Cronitor.Run(MonitorKey);

            VerifySent<RunCommand>("run");
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            await Cronitor.RunAsync(MonitorKey);

            VerifySent<RunCommand>("run");
        }

        [Fact]
        public void ShouldExecuteCompleteMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            Cronitor.Complete(MonitorKey);

            VerifySent<CompleteCommand>("complete");
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            await Cronitor.CompleteAsync(MonitorKey);

            VerifySent<CompleteCommand>("complete");
        }

        [Fact]
        public void ShouldExecuteFailMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            Cronitor.Fail(MonitorKey);

            VerifySent<FailCommand>("fail");
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            await Cronitor.FailAsync(MonitorKey);

            VerifySent<FailCommand>("fail");
        }

        [Fact]
        public void ShouldExecuteTickMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            Cronitor.Tick(MonitorKey);

            VerifySent<TickCommand>("tick");
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);

            await Cronitor.TickAsync(MonitorKey);

            VerifySent<TickCommand>("tick");
        }

        [Fact]
        public void ShouldExecutePingMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            Cronitor.Ping(command);

            VerifySent<CompleteCommand>("complete");
        }

        [Fact]
        public async Task ShouldExecutePingAsyncMethod()
        {
            Cronitor.Configure(ApiKey, _httpClient.Object);
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            await Cronitor.PingAsync(command);

            VerifySent<CompleteCommand>("complete");
        }

        private void VerifySent<TCommand>(string endpoint) where TCommand : Command
        {
            _httpClient.Verify(x => x.SendAsync(It.Is<TCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == endpoint)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}
