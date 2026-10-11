using System;
using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    /// <summary>
    /// A notification Cronitor sent about a monitor: an alert when it started failing, or a recovery.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// "alert" or "recovery".
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
        /// <summary>
        /// How the notification was sent, e.g. "slack" or "email".
        /// </summary>
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
        /// <summary>
        /// Where the notification was sent, e.g. a Slack channel or an email address.
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination { get; set; }
        [JsonPropertyName("monitor")]
        public AlertMonitor Monitor { get; set; }
        /// <summary>
        /// Key of the incident the notification belongs to; an alert and its recovery share it.
        /// </summary>
        [JsonPropertyName("incident")]
        public string Incident { get; set; }
        [JsonPropertyName("notification_list")]
        public string NotificationList { get; set; }
        /// <summary>
        /// Whether the notification was delivered.
        /// </summary>
        [JsonPropertyName("delivered")]
        public bool? Delivered { get; set; }
        [JsonPropertyName("created")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonPropertyName("group_by_key")]
        public string GroupByKey { get; set; }
    }
}
