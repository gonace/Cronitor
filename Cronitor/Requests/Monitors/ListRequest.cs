using Cronitor.Abstractions;

namespace Cronitor.Requests.Monitors
{
    public class ListRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors";
        public int Page { get; set; } = 1;
    }
}
