using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests;

/// <summary>
/// Reads the paging fields of real list responses, which Cronitor names differently per resource.
/// </summary>
public class ListResponseTests
{
    [Theory]
    [JsonData("ListMonitors.json")]
    public void ShouldReadTheTotalOfMonitors(string json)
    {
        var response = Serializer.Deserialize<ListMonitorResponse>(json);

        Assert.Equal(1, response.Total);
    }

    [Theory]
    [JsonData("ListIssues.json")]
    public void ShouldReadTheTotalAndPageCountOfIssues(string json)
    {
        var response = Serializer.Deserialize<ListIssueResponse>(json);

        Assert.Equal(64, response.Total);
        Assert.Equal(32, response.PageCount);
        Assert.Equal(2, response.PageSize);
    }

    [Theory]
    [JsonData("ListNotifications.json")]
    public void ShouldReadTheTotalOfNotifications(string json)
    {
        var response = Serializer.Deserialize<ListNotificationResponse>(json);

        Assert.Equal(1, response.Total);
    }
}
