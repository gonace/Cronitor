using Cronitor.Abstractions;
using Cronitor.Attributes;

namespace Cronitor.Requests
{
    public class ListIssueRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues";
        [QueryStringProperty("page")]
        public int Page { get; set; } = 1;
    }
}
