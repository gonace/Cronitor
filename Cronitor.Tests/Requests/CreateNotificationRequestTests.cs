using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests;

public class CreateNotificationRequestTests
{
    [Theory]
    [JsonData("CreateNotificationRequest.json")]
    public async Task Should_create_a_CreateNotificationRequest_from_a_template(string expected)
    {
        var template = new Notification
        {
            Key = "devops-alerts",
            Name = "DevOps Alerts",
            Notifications = new Notifications
            {
                Emails = new List<string> { "dev@example.com" },
                Slack = new List<string> { "https://hooks.slack.com/test" }
            }
        };
        var request = new CreateNotificationRequest(template);

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
        var request = new CreateNotificationRequest(template);

        Assert.Equal("notifications", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Post()
    {
        var template = new Notification { Key = "test", Name = "Test" };
        var request = new CreateNotificationRequest(template);

        Assert.Equal(HttpMethod.Post, request.Method);
    }

    [Fact]
    public void Should_build_the_uri()
    {
        var template = new Notification { Key = "test", Name = "Test" };
        var request = new CreateNotificationRequest(template);

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/notifications", uri.ToString());
    }
}
