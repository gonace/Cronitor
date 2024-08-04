using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses.Monitors
{
    public class ListResponse : BaseResponse<Monitor>
    {
        [JsonPropertyName("monitors")]
        public override IEnumerable<Monitor> Items { get; set; }
    }
}
