using Cronitor.Models.Monitors;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace Cronitor.Tests
{
    /// <summary>
    /// Reads monitors from a real Cronitor response, so the models are checked against the API's actual format.
    /// </summary>
    public class MonitorDeserializationTests
    {
        [Theory]
        [JsonData("ListMonitors.json")]
        public void ShouldReadTheReadOnlyAttributes(string json)
        {
            var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

            Assert.True(monitor.Passing);
            Assert.False(monitor.Paused);
            Assert.False(monitor.Running);
            Assert.False(monitor.Disabled);
            Assert.True(monitor.Initialized);
            Assert.Equal(1732759200, monitor.ExpectedAt);
            Assert.Equal(new DateTime(2023, 11, 27, 10, 45, 17, DateTimeKind.Utc), monitor.CreatedAt.ToUniversalTime());
        }

        [Theory]
        [JsonData("ListMonitors.json")]
        public void ShouldReadTheLatestEvent(string json)
        {
            var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

            Assert.NotNull(monitor.LatestEvent);
            Assert.Equal("complete", monitor.LatestEvent.Type);
            Assert.Equal(1732675428.482m, monitor.LatestEvent.Timestamp);
        }

        [Theory]
        [JsonData("ListMonitors.json")]
        public void ShouldReadTheGroupNameFromTheAttributes(string json)
        {
            var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

            Assert.Equal("Restore", monitor.GroupName);
            Assert.Equal("Restore", monitor.Attributes.GroupName);
            Assert.Equal("key", monitor.Attributes.Key);
            Assert.Equal("code", monitor.Attributes.Code);
        }

        [Theory]
        [JsonData("ListMonitors.json")]
        public void ShouldReadTheLatestIssue(string json)
        {
            var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

            Assert.NotNull(monitor.LatestIssue);
            Assert.Equal(1711850428, monitor.LatestIssue.Timestamp);
            Assert.Equal("resolved", monitor.LatestIssue.State);
        }

        [Fact]
        public void ShouldSendPausedWhenCreatingAPausedMonitor()
        {
            var monitor = new Job("nightly-backup") { Paused = true };

            var body = Serializer.Serialize(monitor);

            Assert.Contains("\"paused\":true", body);
        }

        [Fact]
        public void ShouldNotSendPausedWhenNotSet()
        {
            var monitor = new Job("nightly-backup");

            var body = Serializer.Serialize(monitor);

            Assert.DoesNotContain("\"paused\":", body);
        }

        [Theory]
        [JsonData("ListMonitors.json")]
        public void ShouldNotSendTheReadOnlyAttributesBack(string json)
        {
            var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

            var body = Serializer.Serialize(monitor);

            Assert.Contains("\"key\":", body);
            Assert.DoesNotContain("\"passing\":", body);
            Assert.DoesNotContain("\"created\":", body);
            Assert.DoesNotContain("\"latest_event\":", body);
            Assert.DoesNotContain("\"next_expected_at\":", body);
            Assert.DoesNotContain("\"attributes\":", body);
            Assert.DoesNotContain("\"group_name\":", body);
            Assert.DoesNotContain("\"latest_issue\":", body);
        }
    }
}
