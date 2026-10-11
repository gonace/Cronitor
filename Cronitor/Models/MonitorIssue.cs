using System.Text.Json.Serialization;

namespace Cronitor.Models;

/// <summary>
/// A summary of a monitor's most recent issue. Which fields are present depends on the API version.
/// </summary>
public class MonitorIssue
{
    /// <summary>
    /// When the issue started, as a Unix timestamp.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("stamp")]
    public long? Timestamp { get; private set; }

    /// <summary>
    /// The issue state, e.g. "unresolved" or "resolved".
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("state")]
    public string State { get; private set; }

    [JsonInclude]
    [JsonPropertyName("key")]
    public string Key { get; private set; }

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; private set; }

    [JsonInclude]
    [JsonPropertyName("severity")]
    public string Severity { get; private set; }
}
