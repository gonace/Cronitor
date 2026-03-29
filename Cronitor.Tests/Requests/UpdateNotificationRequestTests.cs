using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class UpdateNotificationRequestTests
    {
        [Theory]
        [JsonData("UpdateNotificationRequest.json")]
        public async Task ShouldCreateUpdateNotificationRequestAsync(string expected)
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
        public void ShouldSetEndpoint()
        {
            var template = new Notification { Key = "test", Name = "Test" };
            var request = new UpdateNotificationRequest("test", template);

            Assert.Equal("notifications/:key", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToPut()
        {
            var template = new Notification { Key = "test", Name = "Test" };
            var request = new UpdateNotificationRequest("test", template);

            Assert.Equal(HttpMethod.Put, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var template = new Notification { Key = "test", Name = "Test" };
            var request = new UpdateNotificationRequest("test-key", template);

            Assert.Equal("test-key", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var template = new Notification { Key = "test", Name = "Test" };
            var request = new UpdateNotificationRequest("devops-alerts", template);

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/notifications/devops-alerts", uri.ToString());
        }
    }
}
