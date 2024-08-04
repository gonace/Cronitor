using Cronitor.Abstractions;

namespace Cronitor.Requests.Issues
{
    public class ListRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues";
        public int Page { get; set; } = 1;
    }
}
