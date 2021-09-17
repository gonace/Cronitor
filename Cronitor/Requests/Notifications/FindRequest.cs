namespace Cronitor.Requests.Notifications
{
    public class FindRequest : Request
    {
        public override string Endpoint { get; set; } = "templates";
        public int Page { get; set; } = 1;
    }
}
