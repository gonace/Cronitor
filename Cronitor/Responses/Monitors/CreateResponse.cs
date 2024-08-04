using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses.Monitors
{
    public class CreateResponse
    {
        [JsonPropertyName("monitors")]
        public IEnumerable<Models.Monitor> Monitors { get; set; }
    }
}
