using System.Net.Http;
using Cronitor.Constants;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class ListMonitorRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new ListMonitorRequest();

        Assert.Equal("monitors", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Get()
    {
        var request = new ListMonitorRequest();

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_default_Page_to_one()
    {
        var request = new ListMonitorRequest();

        Assert.Equal(1, request.Page);
    }

    [Fact]
    public void Should_build_the_uri_with_the_default_page()
    {
        var request = new ListMonitorRequest();

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1", uri.ToString());
    }

    [Fact]
    public void Should_build_the_uri_with_a_custom_page()
    {
        var request = new ListMonitorRequest { Page = 3 };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=3", uri.ToString());
    }

    [Fact]
    public void Should_build_the_uri_with_a_page_size()
    {
        var request = new ListMonitorRequest { PageSize = 100 };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&pageSize=100", uri.ToString());
    }

    [Fact]
    public void Should_repeat_type_for_every_type()
    {
        var request = new ListMonitorRequest { Types = { MonitorType.Job, MonitorType.Check } };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&type=job&type=check", uri.ToString());
    }

    [Fact]
    public void Should_repeat_tag_for_every_tag()
    {
        var request = new ListMonitorRequest { Tags = { "backups", "nightly" } };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&tag=backups&tag=nightly", uri.ToString());
    }

    [Fact]
    public void Should_repeat_state_for_every_state()
    {
        var request = new ListMonitorRequest { States = { MonitorState.Failing, MonitorState.Paused } };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors?page=1&state=failing&state=paused", uri.ToString());
    }

    [Fact]
    public void Should_build_the_uri_with_every_filter()
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
    public void Should_sort_by_every_documented_order(string sort)
    {
        var sorts = new[] { MonitorSort.CreatedAscending, MonitorSort.CreatedDescending, MonitorSort.NameAscending, MonitorSort.NameDescending };

        Assert.Contains(sorts, s => s.ToString() == sort);
    }
}
