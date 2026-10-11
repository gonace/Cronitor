using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests;

public class UpdateNotificationRequestTests
{
    [Theory]
    [JsonData("UpdateNotificationRequest.json")]
    public async Task Should_create_an_UpdateNotificationRequest(string expected)
    {
        var template = new Notification
        {
            Key = "devops-alerts",
            Name = "DevOps Alerts Updated",
            Notifications = new Notifications
            {
                Emails = new List<string> { "dev@example.com", "ops@example.com" }
            }
        };
        var request = new UpdateNotificationRequest("devops-alerts", template);

#if NET8_0_OR_GREATER
        var actual = await request.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
#else
        var actual = await request.Content.ReadAsStringAsync();
#endif
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_set_Endpoint()
    {
        var template = new Notification { Key = "test", Name = "Test" };
        var request = new UpdateNotificationRequest("test", template);

        Assert.Equal("notifications/:key", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Put()
    {
        var template = new Notification { Key = "test", Name = "Test" };
        var request = new UpdateNotificationRequest("test", template);

        Assert.Equal(HttpMethod.Put, request.Method);
    }

    [Fact]
    public void Should_set_Key()
    {
        var template = new Notification { Key = "test", Name = "Test" };
        var request = new UpdateNotificationRequest("test-key", template);

        Assert.Equal("test-key", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_the_key()
    {
        var template = new Notification { Key = "test", Name = "Test" };
        var request = new UpdateNotificationRequest("devops-alerts", template);

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/notifications/devops-alerts", uri.ToString());
    }
}
