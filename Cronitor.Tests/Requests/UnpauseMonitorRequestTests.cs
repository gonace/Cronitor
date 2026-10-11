using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class UnpauseMonitorRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new UnpauseMonitorRequest("abc123");

        Assert.Equal("monitors/:key/pause/:duration", request.Endpoint);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new UnpauseMonitorRequest("abc123");

        Assert.Equal("abc123", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_a_duration_of_zero()
    {
        var request = new UnpauseMonitorRequest("abc123");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/monitors/abc123/pause/0", uri.ToString());
    }
}
