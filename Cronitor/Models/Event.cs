using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class Event
    {
        [JsonPropertyName("stamp")]
        public decimal Timestamp { get; set; }
        [JsonPropertyName("msg")]
        public string Message { get; set; }
        [JsonPropertyName("event")]
        public string Type { get; set; }
    }
}
