using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    /// <summary>
    /// URLs of embeddable status badges for a monitor.
    /// </summary>
    public class MonitorBadges
    {
        [JsonInclude]
        [JsonPropertyName("simple")]
        public string Simple { get; private set; }

        [JsonInclude]
        [JsonPropertyName("detailed")]
        public string Detailed { get; private set; }
    }
}
