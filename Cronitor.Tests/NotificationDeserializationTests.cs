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

        [Theory]
        [JsonData("ListNotifications.json")]
        public void ShouldReadNoMonitorDetailsForAnUnusedList(string json)
        {
            var notification = Serializer.Deserialize<ListNotificationResponse>(json).Items.Single();

            Assert.Empty(notification.MonitorDetails);
        }

        [Fact]
        public void ShouldReadTheMonitorsUsingTheList()
        {
            const string json = "{\"key\":\"default\",\"monitor_details\":[{\"key\":\"api-health\",\"name\":\"API Health Check\"}]}";

            var monitor = Serializer.Deserialize<Notification>(json).MonitorDetails.Single();

            Assert.Equal("api-health", monitor.Key);
            Assert.Equal("API Health Check", monitor.Name);
        }

        [Fact]
        public void ShouldReadTheEnvironments()
        {
            const string json = "{\"key\":\"default\",\"environments\":[{\"key\":\"production\"}]}";

            var environment = Serializer.Deserialize<Notification>(json).Environments.Single();

            Assert.Equal("production", environment.Key);
        }

        [Fact]
        public void ShouldSendTheEnvironmentsAsObjects()
        {
            var notification = new Notification { Name = "Default", Environments = new[] { new NotificationEnvironment { Key = "production" } } };

            var json = Serializer.Serialize(notification);

            Assert.Contains("\"environments\":[{\"key\":\"production\"}]", json);
        }

        [Theory]
        [JsonData("ListNotifications.json")]
        public void ShouldNotSendWhenTheListWasCreated(string json)
        {
            var notification = Serializer.Deserialize<ListNotificationResponse>(json).Items.Single();

            var sent = Serializer.Serialize(notification);

            Assert.DoesNotContain("\"created\"", sent);
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
