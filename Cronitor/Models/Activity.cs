using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class Activity
    {
        [JsonPropertyName("stamp")]
        public decimal Timestamp { get; set; }
        [JsonPropertyName("monitor_name")]
        public string MonitorName { get; set; }
        [JsonPropertyName("monitor_code")]
        public string MonitorKey { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("duration")]
        public decimal? Duration { get; set; }

        [JsonPropertyName("created")]
        public string CreatedAt { get; set; }
    }
}
