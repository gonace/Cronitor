using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cronitor.Responses.Monitor
{
    public class CreateResponse
    {
        [JsonProperty("monitors")]
        public IEnumerable<Models.Monitor> Monitors { get; set; }
    }
}
