using Newtonsoft.Json;

namespace Cronitor.Models.Monitors
{
    public class Job : Monitor
    {
        [JsonProperty("type")]
        public override string Type { get; set; } = "job";


        public Job()
            : this(GenerateKey())
        {
        }

        public Job(string key)
            : base(key)
        {
        }
    }
}
