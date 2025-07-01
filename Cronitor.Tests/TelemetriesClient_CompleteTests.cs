using Cronitor.Clients;
using Cronitor.Commands;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetriesClient_CompleteTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;
        private readonly TelemetriesClient _telemetriesClient;

        public TelemetriesClient_CompleteTests()
        {
            _httpClient = new Mock<HttpClient>();
            _telemetriesClient = new TelemetriesClient(ApiKey, _httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteCompleteMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Complete(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.CompleteAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Complete(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethodWithMessage()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.CompleteAsync(MonitorKey, message);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithEnvironment()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Complete(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethodWithException()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.CompleteAsync(MonitorKey, environment: Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            _telemetriesClient.Complete(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteCompleteAsyncMethodWithMessageAndEnvironment()
        {
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(Environment);

            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            await _telemetriesClient.CompleteAsync(MonitorKey, message, Environment);

            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}