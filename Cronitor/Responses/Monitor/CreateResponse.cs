using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cronitor.Responses.Monitor
{
    public class CreateResponse
    {
        [JsonProperty("monitors")]
        public IEnumerable<Models.Monitor> Monitors { get; set; }
    }
}
