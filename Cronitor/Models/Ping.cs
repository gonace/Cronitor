using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    /// <summary>
    /// Retrieve correlated ping and alert history in reverse chronological order.
    ///
    /// Note: Old pings are pruned regularly. Note: The values returned will vary based on
    /// the type of monitor. For example, on cron/heartbeat monitors your server IP will
    /// be available in the from field, on healthcheck monitors this field is used
    /// to display the location of our server performing the healthcheck.
    /// </summary>
    public class Ping
    {
        /// <summary>
        /// Decimal timestamp
        /// </summary>
        [JsonPropertyName("stamp")]
        public decimal? Timestamp { get; set; }
        /// <summary>
        /// The display name of the monitor
        /// </summary>
        [JsonPropertyName("monitor_name")]
        public string MonitorName { get; set; }
        /// <summary>
        /// The unique identifier
        /// </summary>
        [JsonPropertyName("monitor_code")]
        public string MonitorKey { get; set; }
        /// <summary>
        /// Enum(alert, healthcheck, ping)
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        /// <summary>
        /// Healthchecks: The location we pinged from
        /// Heartbeat: The IP the ping was received from.
        /// Alert: None.
        /// </summary>
        [JsonPropertyName("from")]
        public string From { get; set; }
        /// <summary>
        /// Healthchecks: The description of the check
        /// Heartbeat: The supplied ?msg, or an empty string.
        /// Alert: Failure description.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
        /// <summary>
        /// Healthchecks: null
        /// Heartbeat: enum(run, complete, fail)
        /// Alert: enum(failure, recovery)
        /// </summary>
        [JsonPropertyName("event")]
        public string Event { get; set; }
        /// <summary>
        /// Healthchecks: The result of the healthcheck
        /// Heartbeat: None
        /// Alert: None
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
        /// <summary>
        /// Healthchecks: The response time of your healthcheck
        /// Heartbeat: None
        /// Alert: None
        /// </summary>
        [JsonPropertyName("duration")]
        public decimal? Duration { get; set; }

        /// <summary>
        /// A datetime display of the provided stamp.
        /// </summary>
        [JsonPropertyName("created")]
        public string CreatedAt { get; set; }
    }
}
