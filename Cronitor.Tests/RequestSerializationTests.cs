using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Models.Monitors;
using Xunit;

namespace Cronitor.Tests;

public class RequestSerializationTests
{
    [Fact]
    public void Should_send_the_url_and_regions_of_a_check()
    {
        var check = new Check("api-health", new Request("https://example.com/health", new[] { Region.California }));

        var json = Serializer.Serialize(check);

        Assert.Contains("\"url\":\"https://example.com/health\"", json);
        Assert.Contains("\"regions\":[\"us-west-1\"]", json);
    }

    [Fact]
    public void Should_send_follow_redirects_and_verify_ssl_when_set()
    {
        var request = new Request("https://example.com/health", new[] { Region.California })
        {
            FollowRedirects = false,
            VerifySsl = true,
        };

        var json = Serializer.Serialize(request);

        Assert.Contains("\"follow_redirects\":false", json);
        Assert.Contains("\"verify_ssl\":true", json);
    }

    [Fact]
    public void Should_not_send_follow_redirects_and_verify_ssl_when_not_set()
    {
        var request = new Request("https://example.com/health", new[] { Region.California });

        var json = Serializer.Serialize(request);

        Assert.DoesNotContain("\"follow_redirects\":", json);
        Assert.DoesNotContain("\"verify_ssl\":", json);
    }

    [Fact]
    public void Should_read_follow_redirects_and_verify_ssl()
    {
        const string json = "{\"url\":\"https://example.com/health\",\"follow_redirects\":true,\"verify_ssl\":false}";

        var request = Serializer.Deserialize<Request>(json);

        Assert.True(request.FollowRedirects);
        Assert.False(request.VerifySsl);
    }

    [Fact]
    public void Should_read_the_url_and_regions_of_a_check()
    {
        const string json = "{\"type\":\"check\",\"key\":\"api-health\",\"request\":{\"url\":\"https://example.com/health\",\"regions\":[\"us-west-1\"]}}";

        var check = Serializer.Deserialize<Check>(json);

        Assert.Equal("https://example.com/health", check.Request.Url);
        Assert.Equal(new[] { "us-west-1" }, check.Request.Regions);
    }
}
