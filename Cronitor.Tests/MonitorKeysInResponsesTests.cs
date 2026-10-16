using Cronitor.Models;
using System.Linq;
using Xunit;

namespace Cronitor.Tests;

public class MonitorKeysInResponsesTests
{
    [Fact]
    public void Should_read_an_activity_s_monitor_key()
    {
        var activity = Serializer.Deserialize<Activity>("{\"monitor_code\":\"reports/daily\"}");

        var key = activity.MonitorKey;

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }

    [Fact]
    public void Should_read_a_ping_s_monitor_key()
    {
        var ping = Serializer.Deserialize<Ping>("{\"monitor_code\":\"reports/daily\"}");

        var key = ping.MonitorKey;

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }

    [Fact]
    public void Should_read_the_key_of_a_monitor_in_an_issue()
    {
        var monitor = Serializer.Deserialize<IssueMonitor>("{\"key\":\"reports/daily\"}");

        var key = monitor.Key;

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }

    [Fact]
    public void Should_read_a_monitor_s_key_in_its_attributes()
    {
        var attributes = Serializer.Deserialize<MonitorAttributes>("{\"key\":\"reports/daily\"}");

        var key = attributes.Key;

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }

    [Fact]
    public void Should_read_the_monitor_keys_of_a_notification_list()
    {
        var notification = Serializer.Deserialize<Notification>("{\"monitors\":[\"reports/daily\"]}");

        var key = notification.Monitors.Single();

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }

    [Fact]
    public void Should_read_the_monitor_of_an_alert()
    {
        var alert = Serializer.Deserialize<Alert>("{\"monitor\":{\"key\":\"reports/daily\",\"name\":\"Daily reports\"}}");

        var key = alert.Monitor.Key;

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }

    [Fact]
    public void Should_read_the_monitor_details_of_a_notification_list()
    {
        var notification = Serializer.Deserialize<Notification>("{\"monitor_details\":[{\"key\":\"reports/daily\",\"name\":\"Daily reports\"}]}");

        var key = notification.MonitorDetails.Single().Key;

        Assert.IsType<MonitorKey>(key);
        Assert.Equal("reports/daily", key.ToString());
    }
}
