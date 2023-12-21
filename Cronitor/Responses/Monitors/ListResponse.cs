using Cronitor.Abstractions;
using Cronitor.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cronitor.Responses.Monitors
{
    public class ListResponse : BaseResponse<Monitor>
    {
        [JsonProperty("monitors")]
        public override IEnumerable<Monitor> Data { get; set; }
    }
}
