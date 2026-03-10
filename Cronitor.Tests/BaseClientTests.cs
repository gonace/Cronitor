using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    // Concrete implementation of BaseClient for testing
    internal class TestClient : BaseClient<TestClient>
    {
        public TestClient(Uri apiUri) : base(apiUri) { }
        public TestClient(Uri apiUri, string apiKey) : base(apiUri, apiKey) { }
        internal TestClient(HttpClient client) : base(client) { }
    }

    public class BaseClientTests : BaseTest
    {
        [Fact]
        public void ShouldConstructWithUriOnly()
        {
            var client = new TestClient(Urls.DefaultApiUrl);

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldConstructWithUriAndApiKey()
        {
            var client = new TestClient(Urls.DefaultApiUrl, ApiKey);

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldConstructWithHttpClient()
        {
            var httpClient = new Mock<HttpClient>();
            var client = new TestClient(httpClient.Object);

            Assert.NotNull(client);
        }

        [Fact]
        public async Task ShouldSendCommandAsync()
        {
            var httpClient = new Mock<HttpClient>();
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            httpClient.Setup(x => x.SendAsync(It.IsAny<Command>())).Returns(Task.CompletedTask);

            var client = new TestClient(httpClient.Object);
            await client.SendAsync(command);

            httpClient.Verify(x => x.SendAsync(It.Is<Command>(c => c == command)), Times.Once);
            httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldSendCommandSync()
        {
            var httpClient = new Mock<HttpClient>();
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(MonitorKey);
            httpClient.Setup(x => x.SendAsync(It.IsAny<Command>())).Returns(Task.CompletedTask);

            var client = new TestClient(httpClient.Object);
            client.Send(command);

            httpClient.Verify(x => x.SendAsync(It.Is<Command>(c => c == command)), Times.Once);
            httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldDisposeWithoutError()
        {
            var client = new TestClient(Urls.DefaultApiUrl, ApiKey);

            client.Dispose();

            // Test passes if no exception is thrown
        }

        [Fact]
        public void ShouldDisposeUsingUsingStatement()
        {
            using (var client = new TestClient(Urls.DefaultApiUrl, ApiKey))
            {
                Assert.NotNull(client);
            }
            // Test passes if no exception is thrown during disposal
        }
    }
}
