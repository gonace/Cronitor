using Newtonsoft.Json;

namespace Cronitor.Models.Monitors
{
    public class Check : Monitor
    {
        [JsonProperty("type")]
        public override string Type { get; set; } = "check";
        [JsonProperty("platform")]
        public override string Platform { get; set; } = "http";
        [JsonProperty("request")]
        public Request Request { get; set; }


        public Check(Request request)
            : this(GenerateKey(), request)
        {
        }

        public Check(string key, Request request)
            : base(key)
        {
            Request = request;
        }
    }
}
