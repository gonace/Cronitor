using Cronitor.Abstractions;

namespace Cronitor.Requests
{
    public class ListIssueRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues";
        public int Page { get; set; } = 1;
    }
}
