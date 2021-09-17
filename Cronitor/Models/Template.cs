using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Template
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("monitors")]
        public IEnumerable<string> Monitors { get; set; }
        [JsonProperty("notifications")]
        public Notifications Notifications { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("created")]
        public DateTime CreatedAt { get; set; }
    }
}
