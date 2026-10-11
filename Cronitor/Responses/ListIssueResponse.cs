using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses;

public class ListIssueResponse : BaseResponse<Issue>
{
    [JsonPropertyName("data")]
    public override IEnumerable<Issue> Items { get; set; }
    [JsonPropertyName("total_count")]
    public override int Total { get; set; }
    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }
}
