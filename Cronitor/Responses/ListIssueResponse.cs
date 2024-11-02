using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses
{
    public class ListIssueResponse : BaseResponse<Issue>
    {
        [JsonPropertyName("data")]
        public override IEnumerable<Issue> Items { get; set; }
    }
}
