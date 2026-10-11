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
        /// <summary>
        /// The host the event was sent from, if one was given.
        /// </summary>
        [JsonPropertyName("host")]
        public string Host { get; set; }
        /// <summary>
        /// The count metric sent with the event, if any.
        /// </summary>
        [JsonPropertyName("count")]
        public decimal? Count { get; set; }
        /// <summary>
        /// The error_count metric sent with the event, if any.
        /// </summary>
        [JsonPropertyName("error_count")]
        public decimal? ErrorCount { get; set; }

        [JsonPropertyName("created")]
        public string CreatedAt { get; set; }
    }
}
