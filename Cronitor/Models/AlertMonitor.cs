using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    /// <summary>
    /// The monitor an <see cref="Alert"/> was sent about.
    /// </summary>
    public class AlertMonitor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
