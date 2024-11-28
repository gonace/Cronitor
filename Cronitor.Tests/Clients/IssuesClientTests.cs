﻿using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Tests.Builders;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class IssuesClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly IssuesClient _issuesClient;

        public IssuesClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _issuesClient = new IssuesClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteListMethod()
        {
            var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListIssueResponse>(It.IsAny<ListIssueRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _issuesClient.List();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListIssueResponse>(It.Is<ListIssueRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "issues")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListIssueResponse>(It.IsAny<ListIssueRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _issuesClient.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListIssueResponse>(It.Is<ListIssueRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "issues")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteListMethodInUsingBlock()
        {
            var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };
            // Setup
            _httpClient.Setup(x => x.SendAsync<ListIssueResponse>(It.IsAny<ListIssueRequest>())).Returns(Task.FromResult(response));

            using (var client = new IssuesClient(_httpClient.Object))
            {
                // Run
                var result = client.List();

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.Items);
                Assert.Equal(1, result.Page);
                Assert.Equal(50, result.PageSize);

                // Verify
                _httpClient.Verify(x => x.SendAsync<ListIssueResponse>(It.Is<ListIssueRequest>(c =>
                    c.Page == 1 &&
                    c.Method == HttpMethod.Get &&
                    c.Endpoint == "issues")), Times.Once);
                _httpClient.VerifyNoOtherCalls();
            }
        }

        [Fact(Skip = "Not implemented yet")]
        public void ShouldExecuteGetMethod()
        {
            var response = Make.Issue.Key(MonitorKey).Build();

            // Setup
            _httpClient.Setup(x => x.SendAsync<Issue>(It.IsAny<GetIssueRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _issuesClient.Get(MonitorKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);

            // Verify
            _httpClient.Verify(x => x.SendAsync<GetIssueRequest>(It.Is<GetIssueRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "issues/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact(Skip = "Not implemented yet")]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = Make.Issue.Key(MonitorKey).Build();

            // Setup
            _httpClient.Setup(x => x.SendAsync<Issue>(It.IsAny<GetIssueRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _issuesClient.GetAsync(MonitorKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);

            // Verify
            _httpClient.Verify(x => x.SendAsync<GetIssueRequest>(It.Is<GetIssueRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "issues/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}
