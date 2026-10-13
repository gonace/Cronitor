using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class GetMonitorRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new GetMonitorRequest("abc123");

        Assert.Equal("monitors/:key", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Get()
    {
        var request = new GetMonitorRequest("abc123");

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new GetMonitorRequest("abc123");

        Assert.Equal("abc123", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_the_key()
    {
        var request = new GetMonitorRequest("abc123");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors/abc123", uri.ToString());
    }

    [Fact]
    public void Should_escape_the_key_in_the_uri()
    {
        var request = new GetMonitorRequest("reports?daily #1");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors/reports%3Fdaily%20%231", uri.AbsoluteUri);
    }
}
