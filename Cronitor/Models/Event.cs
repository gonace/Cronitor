using System.Collections.Generic;
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
        /// <summary>
        /// Metrics sent with the event by name, e.g. "duration", "count" or "error_count".
        /// </summary>
        [JsonPropertyName("metrics")]
        public Dictionary<string, decimal> Metrics { get; set; }
        /// <summary>
        /// What sent the event, e.g. "browser".
        /// </summary>
        [JsonPropertyName("client")]
        public string Client { get; set; }
        /// <summary>
        /// The host the event was sent from, if one was given.
        /// </summary>
        [JsonPropertyName("host")]
        public string Host { get; set; }
        /// <summary>
        /// The IP address the event was received from.
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
