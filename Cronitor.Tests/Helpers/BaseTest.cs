using Cronitor.Constants;
using Cronitor.Models.Monitors;
using System.Collections.Generic;

namespace Cronitor.Tests.Helpers
{
    public class BaseTest
    {
        protected const string ApiKey = "00000000000000000000000000000000";
        protected const string MonitorKey = "AbCdEf";
        protected const string Environment = "Production";
        protected const string TemplateKey = "default";

        protected static Check Check => new Check(new Models.Request("http://www.google.se", new List<Region>
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
        }))
        {
            Schedule = "every 60 seconds",
            Timezone = "Europe/Stockholm"
        };

        protected static Heartbeat Heartbeat => new Heartbeat("every 60 seconds")
        {
            Timezone = "Europe/Stockholm"
        };

        protected static Job Job => new Job
        {
            AlertInterval = "6 hours",
            Assertions = new List<string>
            {
                "metric.duration < 30s",
                "metric.error_count < 5"
            },
            FailureTolerance = 2,
            GraceSeconds = 900,
            Group = "Group",
            Note = "Note",
            Notify = new List<string> { "developers" },
            Platform = "Platform",
            Schedule = "35 0 * * *",
            ScheduleTolerance = 1,
            Tags = new List<string> { "tag", "attribute" },
            Timezone = "Europe/Stockholm"
        };
    }
}
