using Cronitor.Clients;
using Cronitor.Commands;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_RunTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_RunTests()
        {
            _httpClient = new Mock<HttpClient>();
            _telemetriesClient = new TelemetriesClient(ApiKey, _httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteRunMethod()
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

        [Fact]
        public async Task ShouldExecuteRunAsyncMethod()
        {
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.RunAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteRunMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Run(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.RunAsync(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteRunMethodWithEnvironment()
        {
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Run(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Environment == Environment &&
                c.Endpoint == "run")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithEnvironment()
        {
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.RunAsync(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Environment == Environment &&
                c.Endpoint == "run")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteRunMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Run(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.RunAsync(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}