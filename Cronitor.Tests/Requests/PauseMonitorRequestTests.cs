using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class PauseMonitorRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new PauseMonitorRequest("abc123");

        Assert.Equal("monitors/:key/pause", request.Endpoint);
    }

    [Fact]
    public void Should_set_Endpoint_with_a_duration()
    {
        var request = new PauseMonitorRequest("abc123", 60);

        Assert.Equal("monitors/:key/pause/:duration", request.Endpoint);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new PauseMonitorRequest("abc123");

        Assert.Equal("abc123", request.Key);
    }

    [Fact]
    public void Should_set_the_duration()
    {
        var request = new PauseMonitorRequest("abc123", 60);

        Assert.Equal(60, request.Duration);
    }

    [Fact]
    public void Should_not_set_the_duration_when_not_given()
    {
        var request = new PauseMonitorRequest("abc123");

        Assert.Null(request.Duration);
    }

    [Fact]
    public void Should_build_the_uri_without_a_duration()
    {
        var request = new PauseMonitorRequest("abc123");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors/abc123/pause", uri.ToString());
    }

    [Fact]
    public void Should_build_the_uri_with_a_duration()
    {
        var request = new PauseMonitorRequest("abc123", 60);

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors/abc123/pause/60", uri.ToString());
    }
}
