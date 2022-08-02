using System.Threading.Tasks;
using Cronitor.Commands;
using Cronitor.Tests.Helpers;
using Moq;
using Xunit;

namespace Cronitor.Tests
{
    public class TelemetryClientTests : BaseTest
    {
        private readonly Mock<HttpClient> _httpClient;
        private readonly TelemetryClient _client;

        public TelemetryClientTests()
        {
            _httpClient = new Mock<HttpClient>();
            _client = new TelemetryClient(ApiKey, _httpClient.Object);
        }

        #region Run & RunAsync

        [Fact]
        public void ShouldExecuteRunMethod()
        {
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Run(MonitorKey);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.RunAsync(MonitorKey);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteRunMethodWithMessage()
        {
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Run(MonitorKey, message);

            // Verify
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
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.RunAsync(MonitorKey, message);

            // Verify
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
            var environment = "Production";

            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(environment);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Run(MonitorKey, environment: environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Environment == environment &&
                c.Endpoint == "run")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithEnvironment()
        {
            var environment = "Production";

            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(environment);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.RunAsync(MonitorKey, environment: environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Environment == environment &&
                c.Endpoint == "run")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteRunMethodWithMessageAndEnvironment()
        {
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var environment = "Production";

            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(environment);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Run(MonitorKey, message,environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'" &&
                c.Environment == environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteRunAsyncMethodWithMessageAndEnvironment()
        {
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var environment = "Production";

            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message)
                .WithEnvironment(environment);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.RunAsync(MonitorKey, message, environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'"&&
                c.Environment == environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        #endregion

        [Fact]
        public void ShouldExecuteCompleteMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Complete(MonitorKey);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.CompleteAsync(MonitorKey);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteCompleteMethodWithMessage()
        {
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Complete(MonitorKey, message);

            // Verify
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
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.CompleteAsync(MonitorKey, message);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteFailMethod()
        {
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Fail(MonitorKey);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.FailAsync(MonitorKey);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteFailMethodWithMessage()
        {
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Fail(MonitorKey, message);

            // Verify
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
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.FailAsync(MonitorKey, message);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteTickMethod()
        {
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Tick(MonitorKey);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.TickAsync(MonitorKey);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteTickMethodWithMessage()
        {
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _client.Tick(MonitorKey, message);

            // Verify
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
            var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _client.TickAsync(MonitorKey, message);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Message == $"'{message}'")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}
