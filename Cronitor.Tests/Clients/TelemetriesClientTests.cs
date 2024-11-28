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

             // Setup
             _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

             // Run
             _telemetriesClient.Run(MonitorKey);

             // Verify
             _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                 c.ApiKey == ApiKey &&
                 c.MonitorKey == MonitorKey &&
                 c.Endpoint == "run")), Times.Once);
             _httpClient.VerifyNoOtherCalls();
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
            _telemetriesClient.Run(MonitorKey);

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
            await _telemetriesClient.RunAsync(MonitorKey);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Run(MonitorKey, message);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.RunAsync(MonitorKey, message);

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
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithEnvironment(Environment);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Run(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.RunAsync(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Run(MonitorKey, message, Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.RunAsync(MonitorKey, message, Environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<RunCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "run" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        #endregion

        #region Complete & CompleteAsync

        [Fact]
        public void ShouldExecuteCompleteMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Complete(MonitorKey);

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
            await _telemetriesClient.CompleteAsync(MonitorKey);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Complete(MonitorKey, message);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.CompleteAsync(MonitorKey, message);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Complete(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.CompleteAsync(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Complete(MonitorKey, message, Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.CompleteAsync(MonitorKey, message, Environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        #endregion

        #region Fail & FailAsync

        [Fact]
        public void ShouldExecuteFailMethod()
        {
            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Fail(MonitorKey);

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
            await _telemetriesClient.FailAsync(MonitorKey);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Fail(MonitorKey, message);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new FailCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.FailAsync(MonitorKey, message);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Fail(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.FailAsync(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Fail(MonitorKey, message, Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.FailAsync(MonitorKey, message, Environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<FailCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "fail" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        #endregion

        #region Tick & TickAsync

        [Fact]
        public void ShouldExecuteTickMethod()
        {
            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Tick(MonitorKey);

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
            await _telemetriesClient.TickAsync(MonitorKey);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Tick(MonitorKey, message);

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
            const string message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var command = new TickCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey)
                .WithMessage(message);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.TickAsync(MonitorKey, message);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Tick(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.TickAsync(MonitorKey, environment: Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Tick(MonitorKey, message, Environment);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.TickAsync(MonitorKey, message, Environment);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<TickCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "tick" &&
                c.Message == $"'{message}'" &&
                c.Environment == Environment)), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        #endregion

        #region Ping & PingAsync


        [Fact]
        public void ShouldExecutePingMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            _telemetriesClient.Ping(command);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecutePingAsyncMethod()
        {
            var command = new CompleteCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync(command)).Returns(Task.CompletedTask);

            // Run
            await _telemetriesClient.PingAsync(command);

            // Verify
            _httpClient.Verify(x => x.SendAsync(It.Is<CompleteCommand>(c =>
                c.ApiKey == ApiKey &&
                c.MonitorKey == MonitorKey &&
                c.Endpoint == "complete")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        #endregion
    }
}