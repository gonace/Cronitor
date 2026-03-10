using Cronitor.Abstractions;
using Cronitor.Attributes;

namespace Cronitor.Requests
{
    public class ListMonitorRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors";
        [QueryStringProperty("page")]
        public int Page { get; set; } = 1;
    }
}
