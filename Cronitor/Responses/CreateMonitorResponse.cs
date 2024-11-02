using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses
{
    public class CreateMonitorResponse
    {
        [JsonPropertyName("monitors")]
        public IEnumerable<Models.Monitor> Monitors { get; set; }
    }
}
