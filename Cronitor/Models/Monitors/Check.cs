using System.Text.Json.Serialization;

namespace Cronitor.Models.Monitors
{
    public class Check : Monitor
    {
        [JsonPropertyName("type")]
        public override string Type { get; set; } = "check";
        [JsonPropertyName("platform")]
        public override string Platform { get; set; } = "http";
        [JsonPropertyName("request")]
        public Request Request { get; set; }

        public Check(string key, Request request)
            : base(key)
        {
            Request = request;
        }
    }
}
