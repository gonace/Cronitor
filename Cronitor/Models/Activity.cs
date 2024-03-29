﻿using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Activity
    {
        [JsonProperty("stamp")]
        public decimal Timestamp { get; set; }
        [JsonProperty("monitor_name")]
        public string MonitorName { get; set; }
        [JsonProperty("monitor_code")]
        public string MonitorKey { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("duration")]
        public decimal? Duration { get; set; }
        [JsonProperty("created")]
        public string CreatedAt { get; set; }
    }
}
