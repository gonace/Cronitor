using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    /// <summary>
    /// Attributes Cronitor reports about a monitor, such as the display name of its group.
    /// </summary>
    public class MonitorAttributes
    {
        /// <summary>
        /// The display name of the monitor's group, e.g. "Database".
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("group_name")]
        public string GroupName { get; private set; }

        [JsonInclude]
        [JsonPropertyName("key")]
        public string Key { get; private set; }

        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; private set; }
    }
}
