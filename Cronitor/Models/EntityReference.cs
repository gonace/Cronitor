using System.Text.Json.Serialization;

namespace Cronitor.Models;

/// <summary>
/// Points to something in Cronitor, such as a monitor or a group, by its key and name.
/// </summary>
public class EntityReference
{
    [JsonInclude]
    [JsonPropertyName("key")]
    public string Key { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; internal set; }
}
