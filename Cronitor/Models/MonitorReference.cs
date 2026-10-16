using System.Text.Json.Serialization;

namespace Cronitor.Models;

public class MonitorReference
{
    [JsonInclude]
    [JsonPropertyName("key")]
    public MonitorKey Key { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; internal set; }
}
