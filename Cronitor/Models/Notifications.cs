using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    /// <summary>
    /// Where/how you wish to be contacted when alerting is triggered.
    /// The following key/value pairs are all options, at least one of which must not be empty.
    /// Note that notification lists cannot be included recursively. Monitors can, however,
    /// use and extend multiple notification lists. This gives you flexibility to create
    /// focused Notification List that can be composed into your desired notification strategies.
    /// </summary>
    public class Notifications
    {
        /// <summary>
        /// A list of emails to send alerts to.
        /// </summary>
        [JsonPropertyName("emails")]
        public IEnumerable<string> Emails { get; set; }
        [JsonPropertyName("hipchat")]
        public IEnumerable<string> Hipchat { get; set; }
        [JsonPropertyName("microsoft-teams")]
        public IEnumerable<string> MicrosoftTeams { get; set; }
        [JsonPropertyName("opsgenie")]
        public IEnumerable<string> Opsgenie { get; set; }
        /// <summary>
        /// A list of pagerduty keys (found on account settings page).
        /// </summary>
        [JsonPropertyName("pagerduty")]
        public IEnumerable<string> Pagerduty { get; set; }
        /// <summary>
        /// A list of phone numbers to send SMS alerts to.
        /// </summary>
        [JsonPropertyName("phones")]
        public IEnumerable<string> Phones { get; set; }
        /// <summary>
        /// A list of slack webhook URLs (found on account settings page).
        /// </summary>
        [JsonPropertyName("slack")]
        public IEnumerable<string> Slack { get; set; }
        [JsonPropertyName("telegram")]
        public IEnumerable<string> Telegram { get; set; }
        [JsonPropertyName("victorops")]
        public IEnumerable<string> Victorops { get; set; }
        /// <summary>
        /// A list of URLs (prefixed with http:// or https://) to callback to.
        /// </summary>
        [JsonPropertyName("webhooks")]
        public IEnumerable<string> Webhooks { get; set; }
    }
}
