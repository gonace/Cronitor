using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses.Issues
{
    public class ListResponse : BaseResponse<Issue>
    {
        [JsonPropertyName("data")]
        public override IEnumerable<Issue> Items { get; set; }
    }
}
