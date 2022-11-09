using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Models.Monitors;
using Cronitor.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cronitor.Tests
{
    public class MonitorTypeTests : BaseTest
    {
        [Fact]
        public void ShouldCreateCheckMonitor()
        {
            var regions = new List<Region>
            {
                Region.Bahrain,
                Region.California,
                Region.Dublin,
                Region.Frankfurt,
                Region.Mumbai,
                Region.Ohio,
                Region.SaoPaulo,
                Region.Singapore,
                Region.Stockholm,
                Region.Sydney,
                Region.Virginia
            };
            const string schedule = "every 60 seconds";
            const string timezone = "Europe/Stockholm";
            const string url = "http://www.google.se";

            var monitor = new Check(MonitorKey, new Request(url, regions))
            {
                Schedule = schedule,
                Timezone = timezone
            };

            // Assert
            Assert.Equal(MonitorKey, monitor.Key);
            Assert.Equal("check", monitor.Type);

            Assert.Equal("http", monitor.Platform);
            Assert.Equal(timezone, monitor.Timezone);

            Assert.Equal("GET", monitor.Request.Method);
            Assert.Equal(regions.Count, monitor.Request.Regions.Count());
            Assert.Equal(schedule, monitor.Schedule);
            Assert.Equal(10, monitor.Request.TimeoutSeconds);
            Assert.Equal(url, monitor.Request.Url);
        }

        [Fact]
        public void ShouldCreateHeartbeatMonitor()
        {
            const string schedule = "every 60 seconds";
            const string timezone = "Europe/Stockholm";

            var monitor = new Heartbeat(MonitorKey, schedule)
            {
                Timezone = timezone
            };

            // Assert
            Assert.Equal(MonitorKey, monitor.Key);
            Assert.Equal("event", monitor.Type);

            Assert.Equal(schedule, monitor.Schedule);
            Assert.Equal(timezone, monitor.Timezone);
        }

        [Fact]
        public void ShouldCreateJobMonitor()
        {
            var assertions = new List<string>
            {
                "metric.duration < 30s",
                "metric.error_count < 5"
            };
            var notify = new List<string>
            {
                "developers",
                "administrators"
            };
            const string schedule = "every 60 seconds";
            const string timezone = "Europe/Stockholm";

            var monitor = new Job(MonitorKey)
            {
                Assertions = assertions,
                Notify = notify,
                Schedule = schedule,
                Timezone = timezone
            };

            // Assert
            Assert.Equal(MonitorKey, monitor.Key);
            Assert.Equal("job", monitor.Type);

            Assert.Equal(assertions, monitor.Assertions);
            Assert.Equal(notify, monitor.Notify);
            Assert.Equal(schedule, monitor.Schedule);
            Assert.Equal(timezone, monitor.Timezone);
        }
    }
}
