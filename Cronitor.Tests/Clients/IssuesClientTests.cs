using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Requests.Issues;
using Cronitor.Responses.Issues;
using Cronitor.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class IssuesClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly IssuesClient _client;
        public IssuesClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _client = new IssuesClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteListMethod()
        {
            var response = new ListResponse { Items = new List<Issue> { Issue } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListResponse>(It.IsAny<ListRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _client.List();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListResponse>(It.Is<ListRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "issues")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        protected static Issue Issue => new Issue
        {
            Key = TemplateKey,
            Name = TemplateKey,
            CreatedAt = DateTime.Now.AddMinutes(-5),
            UpdatedAt = DateTime.Now
        };
    }
}
