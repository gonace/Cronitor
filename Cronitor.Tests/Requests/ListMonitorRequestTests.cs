using System.Net.Http;
using Cronitor.Constants;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class ListMonitorRequestTests
{
    [Fact]
    public void ShouldSetEndpoint()
    {
        var request = new ListMonitorRequest();

        Assert.Equal("monitors", request.Endpoint);
    }

    [Fact]
    public void ShouldSetHttpMethodToGet()
    {
        var request = new ListMonitorRequest();

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void ShouldDefaultPageToOne()
    {
        var request = new ListMonitorRequest();

        Assert.Equal(1, request.Page);
    }

    [Fact]
    public void ShouldBuildUriWithDefaultPage()
    {
        var request = new ListMonitorRequest();

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1", uri.ToString());
    }

    [Fact]
    public void ShouldBuildUriWithCustomPage()
    {
        var request = new ListMonitorRequest { Page = 3 };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=3", uri.ToString());
    }

    [Fact]
    public void ShouldBuildUriWithPageSize()
    {
        var request = new ListMonitorRequest { PageSize = 100 };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&pageSize=100", uri.ToString());
    }

    [Fact]
    public void ShouldRepeatTypeForEveryType()
    {
        var request = new ListMonitorRequest { Types = { MonitorType.Job, MonitorType.Check } };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&type=job&type=check", uri.ToString());
    }

    [Fact]
    public void ShouldRepeatTagForEveryTag()
    {
        var request = new ListMonitorRequest { Tags = { "backups", "nightly" } };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&tag=backups&tag=nightly", uri.ToString());
    }

    [Fact]
    public void ShouldRepeatStateForEveryState()
    {
        var request = new ListMonitorRequest { States = { MonitorState.Failing, MonitorState.Paused } };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&state=failing&state=paused", uri.ToString());
    }

    [Fact]
    public void ShouldBuildUriWithEveryFilter()
    {
        var request = new ListMonitorRequest
        {
            Page = 2,
            PageSize = 25,
            Types = { MonitorType.Job },
            Tags = { "backups" },
            States = { MonitorState.Failing },
            Group = "production-jobs",
            Environment = "production",
            Search = "backup",
            Sort = MonitorSort.NameDescending,
        };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=2&pageSize=25&type=job&tag=backups&state=failing&group=production-jobs&env=production&search=backup&sort=-name", uri.ToString());
    }

    [Theory]
    [InlineData("created")]
    [InlineData("-created")]
    [InlineData("name")]
    [InlineData("-name")]
    public void ShouldSortByEveryDocumentedOrder(string sort)
    {
        var sorts = new[] { MonitorSort.CreatedAscending, MonitorSort.CreatedDescending, MonitorSort.NameAscending, MonitorSort.NameDescending };

        Assert.Contains(sorts, s => s.ToString() == sort);
    }
}
