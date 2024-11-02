using Cronitor.Abstractions;

namespace Cronitor.Requests
{
    public class ListMonitorRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors";
        public int Page { get; set; } = 1;
    }
}
