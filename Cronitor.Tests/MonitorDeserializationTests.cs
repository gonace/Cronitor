using Cronitor.Models.Monitors;
using Cronitor.Responses;
using Cronitor.Scheduling;
using Cronitor.Tests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace Cronitor.Tests;

/// <summary>
/// Reads monitors from a real Cronitor response, so the models are checked against the API's actual format.
/// </summary>
public class MonitorDeserializationTests
{
    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_read_only_attributes(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.True(monitor.Passing);
        Assert.False(monitor.Paused);
        Assert.False(monitor.Running);
        Assert.False(monitor.Disabled);
        Assert.True(monitor.Initialized);
        Assert.Equal(1_732_759_200, monitor.ExpectedAt);
        Assert.Equal(new DateTime(2023, 11, 27, 10, 45, 17, DateTimeKind.Utc), monitor.CreatedAt.ToUniversalTime());
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_latest_event(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.NotNull(monitor.LatestEvent);
        Assert.Equal("complete", monitor.LatestEvent.Type);
        Assert.Equal(1732675428.482m, monitor.LatestEvent.Timestamp);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_latest_event_details(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.Equal(49.243m, monitor.LatestEvent.Metrics["duration"]);
        Assert.Equal("browser", monitor.LatestEvent.Client);
        Assert.Null(monitor.LatestEvent.Host);
        Assert.Equal("52.169.185.15", monitor.LatestEvent.Ip);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_group_name_from_the_attributes(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.Equal("Restore", monitor.GroupName);
        Assert.Equal("Restore", monitor.Attributes.GroupName);
        Assert.Equal("key", monitor.Attributes.Key);
        Assert.Equal("code", monitor.Attributes.Code);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_latest_issue(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.NotNull(monitor.LatestIssue);
        Assert.Equal(1_711_850_428, monitor.LatestIssue.Timestamp);
        Assert.Equal("resolved", monitor.LatestIssue.State);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_schedules(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.Equal(new[] { "0 3 * * *" }, monitor.Schedules.Select(s => s.Value));
        Assert.Equal("0 3 * * *", monitor.Schedule.Value);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_badges(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.Equal("https://cronitor.io/badges/key/production/hash.svg", monitor.Badges.Simple);
        Assert.Equal("https://cronitor.io/badges/key/production/hash/detailed.svg", monitor.Badges.Detailed);
    }

    [Fact]
    public void Should_send_a_single_schedule_as_schedules()
    {
        var monitor = new Job("nightly-backup") { Schedule = Schedule.Cron.Daily(hour: 3) };

        var body = Serializer.Serialize(monitor);

        Assert.Contains("\"schedules\":[\"0 3 * * *\"]", body);
        Assert.DoesNotContain("\"schedule\":", body);
    }

    [Fact]
    public void Should_send_every_schedule()
    {
        var monitor = new Job("nightly-backup") { Schedules = new[] { Schedule.Cron.Daily(hour: 3), Schedule.Cron.Daily(hour: 15) } };

        var body = Serializer.Serialize(monitor);

        Assert.Contains("\"schedules\":[\"0 3 * * *\",\"0 15 * * *\"]", body);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_read_the_environments_status_pages_and_alert_threshold(string json)
    {
        var monitor = Serializer.Deserialize<ListMonitorResponse>(json).Items.Single();

        Assert.Equal(new[] { "production" }, monitor.Environments);
        Assert.Empty(monitor.StatusPages);
        Assert.Equal(10, monitor.ConsecutiveAlertThreshold);
    }

    [Fact]
    public void Should_read_the_metric_names()
    {
        const string json = "{\"type\":\"job\",\"key\":\"nightly-backup\",\"metric_names\":[\"rows_processed\",\"queue_depth\"]}";

        var monitor = Serializer.Deserialize<Job>(json);

        Assert.Equal(new[] { "rows_processed", "queue_depth" }, monitor.MetricNames);
    }

    [Fact]
    public void Should_send_the_environments_status_pages_and_alert_threshold()
    {
        var monitor = new Job("nightly-backup")
        {
            Environments = new[] { "production" },
            StatusPages = new[] { "public" },
            ConsecutiveAlertThreshold = 5,
        };

        var body = Serializer.Serialize(monitor);

        Assert.Contains("\"environments\":[\"production\"]", body);
        Assert.Contains("\"statuspages\":[\"public\"]", body);
        Assert.Contains("\"consecutive_alert_threshold\":5", body);
    }

    [Fact]
    public void Should_send_paused_when_creating_a_paused_monitor()
    {
        var monitor = new Job("nightly-backup") { Paused = true };

        var body = Serializer.Serialize(monitor);

        Assert.Contains("\"paused\":true", body);
    }

    [Fact]
    public void Should_not_send_paused_when_not_set()
    {
        var monitor = new Job("nightly-backup");

        var body = Serializer.Serialize(monitor);

        Assert.DoesNotContain("\"paused\":", body);
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_not_send_the_read_only_attributes_back(string json)
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
        Assert.DoesNotContain("\"badges\":", body);
        Assert.DoesNotContain("\"metric_names\":", body);
    }
}
