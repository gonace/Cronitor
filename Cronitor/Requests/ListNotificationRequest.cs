using Cronitor.Abstractions;

namespace Cronitor.Requests
{
    public class ListNotificationRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "templates";
        public int Page { get; set; } = 1;
    }
}
