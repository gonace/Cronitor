using Cronitor.Attributes;
using Cronitor.Constants;
using System;
using System.Text.Json.Serialization;

namespace Cronitor.Models.Issues;

public class Event
{
    /// <summary>
    /// The unique identifier for this event.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("key")]
    public string Key { get; internal set; }
    /// <summary>
    /// An ISO 8601 formatted timestamp of when the issue started,
    /// e.g. 2023-06-02T09:39:31Z
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; internal set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    /// <summary>
    /// The current state of the issue. Used within the Cronitor
    /// application, and, if published, on your status page(s).
    /// </summary>
    [JsonPropertyName("state")]
    public IssueState State { get; set; }
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("created_by")]
    public Author CreatedBy { get; internal set; }
    /// <summary>
    /// ISO 8601 formatted timestamp of when the issue was created.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("created")]
    public DateTime CreatedAt { get; internal set; }
    /// <summary>
    /// ISO 8601 formatted timestamp of when the issue was created.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("updated")]
    public DateTime? UpdatedAt { get; internal set; }
}
