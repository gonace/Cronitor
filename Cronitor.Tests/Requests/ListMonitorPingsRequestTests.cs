using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class ListMonitorPingsRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new ListPingsRequest("abc123");

        Assert.Equal("monitors/:key/pings", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Get()
    {
        var request = new ListPingsRequest("abc123");

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new ListPingsRequest("abc123");

        Assert.Equal("abc123", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_the_key()
    {
        var request = new ListPingsRequest("abc123");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors/abc123/pings", uri.ToString());
    }
}
