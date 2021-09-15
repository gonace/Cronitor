using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Incident
    {
        [JsonProperty("stamp")]
        public decimal Timestamp { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
    }
}
