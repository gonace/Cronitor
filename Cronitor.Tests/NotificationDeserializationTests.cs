using Cronitor.Models;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using System.Linq;
using Xunit;

namespace Cronitor.Tests
{
    /// <summary>
    /// Reads notification lists from a real Cronitor response, so the models are checked against the API's actual format.
    /// </summary>
    public class NotificationDeserializationTests
    {
        [Theory]
        [JsonData("ListNotifications.json")]
        public void ShouldReadTheNewerChannels(string json)
        {
            var notifications = Serializer.Deserialize<ListNotificationResponse>(json).Items.Single().Notifications;

            Assert.Empty(notifications.Discord);
            Assert.Empty(notifications.GoogleChat);
            Assert.Empty(notifications.Lark);
            Assert.Empty(notifications.DatadogOnCall);
        }

        [Fact]
        public void ShouldSendTheNewerChannels()
        {
            var notifications = new Notifications
            {
                Discord = new[] { "discord-alerts" },
                GoogleChat = new[] { "gchat-alerts" },
                Lark = new[] { "lark-alerts" },
                DatadogOnCall = new[] { "datadog-alerts" }
            };

            var json = Serializer.Serialize(notifications);

            Assert.Contains("\"discord\":[\"discord-alerts\"]", json);
            Assert.Contains("\"gchat\":[\"gchat-alerts\"]", json);
            Assert.Contains("\"larksuite\":[\"lark-alerts\"]", json);
            Assert.Contains("\"datadog-on-call\":[\"datadog-alerts\"]", json);
        }
    }
}
