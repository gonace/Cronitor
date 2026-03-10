using Cronitor.Abstractions;
using Cronitor.Attributes;

namespace Cronitor.Requests
{
    public class ListNotificationRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "templates";
        [QueryStringProperty("page")]
        public int Page { get; set; } = 1;
    }
}
