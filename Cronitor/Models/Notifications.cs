using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Notifications
    {
        [JsonProperty("emails")]
        public IEnumerable<string> Emails { get; set; }
        [JsonProperty("hipchat")]
        public IEnumerable<string> Hipchat { get; set; }
        [JsonProperty("microsoft-teams")]
        public IEnumerable<string> MicrosoftTeams { get; set; }
        [JsonProperty("opsgenie")]
        public IEnumerable<string> Opsgenie { get; set; }
        [JsonProperty("pagerduty")]
        public IEnumerable<string> Pagerduty { get; set; }
        [JsonProperty("phones")]
        public IEnumerable<string> Phones { get; set; }
        [JsonProperty("slack")]
        public IEnumerable<string> Slack { get; set; }
        [JsonProperty("telegram")]
        public IEnumerable<string> Telegram { get; set; }
        [JsonProperty("victorops")]
        public IEnumerable<string> Victorops { get; set; }
        [JsonProperty("webhooks")]
        public IEnumerable<string> Webhooks { get; set; }
    }
}
