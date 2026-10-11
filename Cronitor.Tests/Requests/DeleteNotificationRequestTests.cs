using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class DeleteNotificationRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new DeleteNotificationRequest("default");

        Assert.Equal("notifications/:key", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Delete()
    {
        var request = new DeleteNotificationRequest("default");

        Assert.Equal(HttpMethod.Delete, request.Method);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new DeleteNotificationRequest("default");

        Assert.Equal("default", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_the_key()
    {
        var request = new DeleteNotificationRequest("devops-alerts");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/notifications/devops-alerts", uri.ToString());
    }
}
