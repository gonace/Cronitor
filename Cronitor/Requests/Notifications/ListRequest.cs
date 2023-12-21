using Cronitor.Abstractions;

namespace Cronitor.Requests.Notifications
{
    public class ListRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "templates";
        public int Page { get; set; } = 1;
    }
}
