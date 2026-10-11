using System.Text.Json.Serialization;

namespace Cronitor.Models;

/// <summary>
/// An environment a <see cref="Notification"/> list is limited to.
/// </summary>
public class NotificationEnvironment
{
    /// <summary>
    /// The key of the environment, e.g. "production".
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }
}
