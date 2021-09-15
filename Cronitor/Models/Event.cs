using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Event
    {
        [JsonProperty("stamp")]
        public decimal Timestamp { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("event")]
        public string Type { get; set; }
    }
}
