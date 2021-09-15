using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Activity
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("stamp")]
        public decimal Timestamp { get; set; }
        [JsonProperty("monitor_code")]
        public string MonitorKey { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
