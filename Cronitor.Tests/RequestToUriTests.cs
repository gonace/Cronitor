using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests;

public class RequestToUriTests : BaseTest
{
    [Fact]
    public void Should_replace_the_key_placeholder_in_the_PauseMonitorRequest_uri()
    {
        var request = new PauseMonitorRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}/pause", uri.ToString());
    }

    [Fact]
    public void Should_include_the_duration_in_the_PauseMonitorRequest_uri()
    {
        var request = new PauseMonitorRequest(MonitorKey, 24);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}/pause/24", uri.ToString());
    }

    [Fact]
    public void Should_include_a_duration_of_zero_in_the_UnpauseMonitorRequest_uri()
    {
        var request = new UnpauseMonitorRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}/pause/0", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_GetMonitorRequest_uri()
    {
        var request = new GetMonitorRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_DeleteMonitorRequest_uri()
    {
        var request = new DeleteMonitorRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_ListActivitiesRequest_uri()
    {
        var request = new ListActivitiesRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}/activity", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_ListAlertsRequest_uri()
    {
        var request = new ListAlertsRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}/alerts", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_ListPingsRequest_uri()
    {
        var request = new ListPingsRequest(MonitorKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"monitors/{MonitorKey}/pings", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_GetIssueRequest_uri()
    {
        const string issueKey = "issue-123";
        var request = new GetIssueRequest(issueKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"issues/{issueKey}", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_UpdateIssueRequest_uri()
    {
        const string issueKey = "issue-123";
        var request = new UpdateIssueRequest(issueKey, new Issue());
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"issues/{issueKey}", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_GetNotificationRequest_uri()
    {
        var request = new GetNotificationRequest(TemplateKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"notifications/{TemplateKey}", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_DeleteNotificationRequest_uri()
    {
        var request = new DeleteNotificationRequest(TemplateKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"notifications/{TemplateKey}", uri.ToString());
    }

    [Fact]
    public void Should_replace_the_key_placeholder_in_the_UpdateNotificationRequest_uri()
    {
        var request = new UpdateNotificationRequest(TemplateKey, new Notification());
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains($"notifications/{TemplateKey}", uri.ToString());
    }

    [Fact]
    public void Should_include_the_page_in_the_ListMonitorRequest_query_string()
    {
        var request = new ListMonitorRequest { Page = 2 };
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains("monitors", uri.ToString());
        Assert.Contains("page=2", uri.ToString());
    }

    [Fact]
    public void Should_include_the_page_in_the_ListIssueRequest_query_string()
    {
        var request = new ListIssueRequest { Page = 3 };
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains("issues", uri.ToString());
        Assert.Contains("page=3", uri.ToString());
    }

    [Fact]
    public void Should_encode_special_characters_in_the_PauseMonitorRequest_uri()
    {
        const string specialKey = "monitor-with-special-chars";
        var request = new PauseMonitorRequest(specialKey);
        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains(specialKey, uri.ToString());
    }
}
