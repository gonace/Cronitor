using System.Text.Json.Serialization;

namespace Cronitor.Models;

/// <summary>
/// A monitor affected by an <see cref="Issue"/>, as Cronitor lists it on the issue.
/// </summary>
public class IssueMonitor
{
    [JsonInclude]
    [JsonPropertyName("key")]
    public string Key { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; internal set; }
    /// <summary>
    /// The kind of monitor, e.g. "job", "check" or "heartbeat".
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("type")]
    public string Type { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("passing")]
    public bool? Passing { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("paused")]
    public bool? Paused { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("active")]
    public bool? Active { get; internal set; }
    [JsonInclude]
    [JsonPropertyName("group")]
    public EntityReference Group { get; internal set; }
}
