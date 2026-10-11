using Cronitor.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Models;

public class Notification
{
    /// <summary>
    /// The unique identifier for this list.
    /// May contain letters, numbers, dashes, and underscores.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }
    /// <summary>
    /// The name of your Notification List
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    /// <summary>
    /// A list of monitors attached to this Notification List.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("monitors")]
    public IEnumerable<string> Monitors { get; internal set; }
    /// <summary>
    /// The key and name of each monitor attached to this Notification List.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("monitor_details")]
    public IEnumerable<NotificationMonitor> MonitorDetails { get; private set; }
    /// <summary>
    /// Where/how you wish to be contacted when alerting is triggered.
    /// The following key/value pairs are all options, at least one of which must not be empty.
    /// Note that notification lists cannot be included recursively. Monitors can, however,
    /// use and extend multiple notification lists. This gives you flexibility to create
    /// focused Notification List that can be composed into your desired notification strategies.
    /// </summary>
    [JsonPropertyName("notifications")]
    public Notifications Notifications { get; set; }
    /// <summary>
    /// The environments this Notification List is limited to.
    /// Leave empty to alert in every environment.
    /// </summary>
    [JsonPropertyName("environments")]
    public IEnumerable<NotificationEnvironment> Environments { get; set; }
    /// <summary>
    /// A plain text status description of this monitor
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("status")]
    public string Status { get; internal set; }

    /// <summary>
    /// ISO formatted string of when the Notification
    /// List was created
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("created")]
    public DateTime CreatedAt { get; internal set; }
}
