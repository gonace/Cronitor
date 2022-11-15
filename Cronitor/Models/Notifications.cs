using Newtonsoft.Json;
using System.Collections.Generic;

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
        [JsonProperty("emails")]
        public IEnumerable<string> Emails { get; set; }
        [JsonProperty("hipchat")]
        public IEnumerable<string> Hipchat { get; set; }
        [JsonProperty("microsoft-teams")]
        public IEnumerable<string> MicrosoftTeams { get; set; }
        [JsonProperty("opsgenie")]
        public IEnumerable<string> Opsgenie { get; set; }
        /// <summary>
        /// A list of pagerduty keys (found on account settings page).
        /// </summary>
        [JsonProperty("pagerduty")]
        public IEnumerable<string> Pagerduty { get; set; }
        /// <summary>
        /// A list of phone numbers to send SMS alerts to.
        /// </summary>
        [JsonProperty("phones")]
        public IEnumerable<string> Phones { get; set; }
        /// <summary>
        /// A list of slack webhook URLs (found on account settings page).
        /// </summary>
        [JsonProperty("slack")]
        public IEnumerable<string> Slack { get; set; }
        [JsonProperty("telegram")]
        public IEnumerable<string> Telegram { get; set; }
        [JsonProperty("victorops")]
        public IEnumerable<string> Victorops { get; set; }
        /// <summary>
        /// A list of URLs (prefixed with http:// or https://) to callback to.
        /// </summary>
        [JsonProperty("webhooks")]
        public IEnumerable<string> Webhooks { get; set; }
    }
}
