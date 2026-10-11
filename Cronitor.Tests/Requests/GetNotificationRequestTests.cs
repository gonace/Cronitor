using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class GetNotificationRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new GetNotificationRequest("default");

        Assert.Equal("notifications/:key", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Get()
    {
        var request = new GetNotificationRequest("default");

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new GetNotificationRequest("default");

        Assert.Equal("default", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_the_key()
    {
        var request = new GetNotificationRequest("devops-alerts");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/notifications/devops-alerts", uri.ToString());
    }
}
