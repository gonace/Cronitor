using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class ListNotificationRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new ListNotificationRequest();

        Assert.Equal("notifications", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Get()
    {
        var request = new ListNotificationRequest();

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_default_Page_to_one()
    {
        var request = new ListNotificationRequest();

        Assert.Equal(1, request.Page);
    }

    [Fact]
    public void Should_build_the_uri_with_the_default_page()
    {
        var request = new ListNotificationRequest();

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/notifications?page=1", uri.ToString());
    }

    [Fact]
    public void Should_build_the_uri_with_a_custom_page()
    {
        var request = new ListNotificationRequest { Page = 5 };

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/notifications?page=5", uri.ToString());
    }
}
