using System.Text.Json.Serialization;

namespace Cronitor.Models.Monitors
{
    public class Job : Monitor
    {
        [JsonPropertyName("type")]
        public override string Type { get; set; } = "job";

        public Job(string key)
            : base(key)
        {
        }
    }
}
