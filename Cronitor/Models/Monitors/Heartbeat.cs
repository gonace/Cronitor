using System.Text.Json.Serialization;

namespace Cronitor.Models.Monitors
{
    public class Heartbeat : Monitor
    {
        [JsonPropertyName("type")]
        public override string Type { get; set; } = "event";

        public Heartbeat(string schedule)
            : this(GenerateKey(), schedule)
        {
        }

        public Heartbeat(string key, string schedule)
            : base(key)
        {
            Schedule = schedule;
        }
    }
}
