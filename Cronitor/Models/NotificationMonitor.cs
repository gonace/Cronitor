using System.Text.Json.Serialization;

namespace Cronitor.Models;

/// <summary>
/// A monitor that sends its alerts to a <see cref="Notification"/> list.
/// </summary>
public class NotificationMonitor
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
