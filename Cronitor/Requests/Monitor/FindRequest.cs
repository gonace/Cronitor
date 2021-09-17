namespace Cronitor.Requests.Monitor
{
    public class FindRequest : Request
    {
        public override string Endpoint { get; set; } = "monitors";
        public int Page { get; set; } = 1;
    }
}
