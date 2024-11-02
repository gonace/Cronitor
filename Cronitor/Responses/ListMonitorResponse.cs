using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses
{
    public class ListMonitorResponse : BaseResponse<Monitor>
    {
        [JsonPropertyName("monitors")]
        public override IEnumerable<Monitor> Items { get; set; }
    }
}
