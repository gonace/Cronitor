using Cronitor.Clients;
using Cronitor.Commands;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class TelemetriesClient_TickTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_TickTests()
        {
            _httpClient = new Mock<HttpClient>();
            _telemetriesClient = new TelemetriesClient(ApiKey, _httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteTickMethod()
        {
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Tick(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethod()
        {
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.TickAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteTickMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Tick(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.TickAsync(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteTickMethodWithEnvironment()
        {
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Tick(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethodWithEnvironment()
        {
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.TickAsync(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteTickMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Tick(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteTickAsyncMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.TickAsync(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}