using Cronitor.Clients;
using Cronitor.Commands;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_FailTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_FailTests()
        {
            _httpClient = new Mock<HttpClient>();
            _telemetriesClient = new TelemetriesClient(ApiKey, _httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteFailMethod()
        {
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Fail(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethod()
        {
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.FailAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteFailMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Fail(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.FailAsync(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteFailMethodWithEnvironment()
        {
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Fail(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethodWithEnvironment()
        {
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.FailAsync(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteFailMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Fail(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteFailAsyncMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.FailAsync(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}