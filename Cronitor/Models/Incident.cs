using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class Incident
    {
        [JsonPropertyName("stamp")]
        public decimal Timestamp { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
    }
}
