using System.Text.Json.Serialization;
using Cronitor.Constants.Scheduling;

namespace Cronitor.Models.Monitors
{
    public class Heartbeat : Monitor
    {
        [JsonPropertyName("type")]
        public override string Type { get; set; } = "event";

        public Heartbeat(string key, ScheduleExpression schedule)
            : base(key)
        {
            Schedule = schedule;
        }
    }
}
