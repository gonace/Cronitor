using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Builders;
using Cronitor.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

public class IssuesClientTests : BaseTest
{
    private const string IssuesUri = "https://cronitor.io/api/issues";
    private const string IssueUri = IssuesUri + "/" + MonitorKey;

    [Fact]
    public void ShouldConstructWithApiKey()
    {
        var client = new IssuesClient(ApiKey);

        Assert.NotNull(client);
    }

    [Fact]
    public void ShouldDisposeIssuesClient()
    {
        using (var client = new IssuesClient(ApiKey))
        {
            Assert.NotNull(client);
        }
        // Test passes if no exception is thrown during disposal
    }

    [Fact]
    public void ShouldExecuteListMethod()
    {
        var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var issuesClient = CreateClient(handler);

        var result = issuesClient.List();

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
        AssertRequest.Sent(handler, HttpMethod.Get, IssuesUri + "?page=1");
    }

    [Fact]
    public async Task ShouldExecuteListAsyncMethod()
    {
        var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var issuesClient = CreateClient(handler);

        var result = await issuesClient.ListAsync();

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
        AssertRequest.Sent(handler, HttpMethod.Get, IssuesUri + "?page=1");
    }

    [Fact]
    public void ShouldExecuteListMethodInUsingBlock()
    {
        var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));

        using (var client = CreateClient(handler))
        {
            var result = client.List();

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            AssertRequest.Sent(handler, HttpMethod.Get, IssuesUri + "?page=1");
        }
    }

    [Fact]
    public void ShouldExecuteListMethodWithFilters()
    {
        var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var issuesClient = CreateClient(handler);

        var result = issuesClient.List(new ListIssueRequest { States = { IssueState.Unresolved }, Time = "24h" });

        Assert.Single(result.Items);
        AssertRequest.Sent(handler, HttpMethod.Get, IssuesUri + "?page=1&state=unresolved&time=24h");
    }

    [Fact]
    public async Task ShouldExecuteListAsyncMethodWithFilters()
    {
        var response = new ListIssueResponse { Items = new List<Issue> { Make.Issue.Build() } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var issuesClient = CreateClient(handler);

        var result = await issuesClient.ListAsync(new ListIssueRequest { Jobs = { MonitorKey }, OrderBy = IssueOrder.StartedDescending });

        Assert.Single(result.Items);
        AssertRequest.Sent(handler, HttpMethod.Get, IssuesUri + $"?page=1&job={MonitorKey}&orderBy=-started");
    }

    [Fact]
    public async Task ListAsyncShouldRequireARequest()
    {
        var issuesClient = CreateClient(new FakeHttpMessageHandler());

        await Assert.ThrowsAsync<ArgumentNullException>(() => issuesClient.ListAsync((ListIssueRequest)null));
    }

    [Fact]
    public void ShouldExecuteGetMethod()
    {
        var response = Make.Issue.Key(MonitorKey).Build();
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var issuesClient = CreateClient(handler);

        var result = issuesClient.Get(MonitorKey);

        Assert.NotNull(result);
        Assert.Equal(response.Key, result.Key);
        AssertRequest.Sent(handler, HttpMethod.Get, IssueUri);
    }

    [Fact]
    public async Task ShouldExecuteGetAsyncMethod()
    {
        var response = Make.Issue.Key(MonitorKey).Build();
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var issuesClient = CreateClient(handler);

        var result = await issuesClient.GetAsync(MonitorKey);

        Assert.NotNull(result);
        Assert.Equal(response.Key, result.Key);
        AssertRequest.Sent(handler, HttpMethod.Get, IssueUri);
    }

    [Fact]
    public void ShouldExecuteCreateMethod()
    {
        var issue = Make.Issue.Build();
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(issue));
        var issuesClient = CreateClient(handler);

        var result = issuesClient.Create(new CreateIssueRequest(issue));

        Assert.NotNull(result);
        AssertRequest.Sent(handler, HttpMethod.Post, IssuesUri);
    }

    [Fact]
    public async Task ShouldExecuteCreateAsyncMethod()
    {
        var issue = Make.Issue.Build();
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(issue));
        var issuesClient = CreateClient(handler);

        var result = await issuesClient.CreateAsync(new CreateIssueRequest(issue));

        Assert.NotNull(result);
        AssertRequest.Sent(handler, HttpMethod.Post, IssuesUri);
    }

    [Fact]
    public void ShouldExecuteUpdateMethod()
    {
        var issue = Make.Issue.Key(MonitorKey).Build();
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(issue));
        var issuesClient = CreateClient(handler);

        var result = issuesClient.Update(new UpdateIssueRequest(MonitorKey, issue));

        Assert.NotNull(result);
        Assert.Equal(MonitorKey, result.Key);
        AssertRequest.Sent(handler, HttpMethod.Put, IssueUri);
    }

    [Fact]
    public async Task ShouldExecuteUpdateAsyncMethod()
    {
        var issue = Make.Issue.Key(MonitorKey).Build();
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(issue));
        var issuesClient = CreateClient(handler);

        var result = await issuesClient.UpdateAsync(new UpdateIssueRequest(MonitorKey, issue));

        Assert.NotNull(result);
        Assert.Equal(MonitorKey, result.Key);
        AssertRequest.Sent(handler, HttpMethod.Put, IssueUri);
    }

    private static IssuesClient CreateClient(FakeHttpMessageHandler handler)
    {
        return new IssuesClient(new Internals.HttpClient(Urls.DefaultApiUrl, ApiKey, handler));
    }
}
