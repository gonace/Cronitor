using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Requests.Notifications;
using Cronitor.Responses.Notifications;
using Cronitor.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class NotificationsClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly NotificationsClient _client;

        public NotificationsClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _client = new NotificationsClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteListMethod()
        {
            var response = new ListResponse { Items = new List<Template> { Template } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListResponse>(It.IsAny<ListRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _client.List();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListResponse>(It.Is<ListRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListResponse { Items = new List<Template> { Template } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListResponse>(It.IsAny<ListRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _client.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListResponse>(It.Is<ListRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteGetMethod()
        {
            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<GetRequest>())).Returns(Task.FromResult(Template));

            // Run
            var result = _client.Get(TemplateKey);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<GetRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<GetRequest>())).Returns(Task.FromResult(Template));

            // Run
            var result = await _client.GetAsync(TemplateKey);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<GetRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        protected static Template Template => new Template
        {
            Key = TemplateKey,
            Monitors = new List<string> { MonitorKey },
            CreatedAt = DateTime.Now,
            Name = TemplateKey,
            Notifications = new Notifications
            {
                Emails = new List<string> { "jane.doe@domain.tld", "john.doe@domain.tld" }
            },
            Status = "0 recipients used by 5 monitors"
        };
    }
}
