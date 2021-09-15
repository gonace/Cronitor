namespace Cronitor.Requests.Monitor
{
    public class GetRequest : Request
    {
        public override string Endpoint { get; set; } = "monitors/:monitorKey";
        public string MonitorKey { get; set; }

        public GetRequest(string monitorKey)
        {
            MonitorKey = monitorKey;
        }

        public override string ToUrl()
        {
            return Endpoint.Replace(":monitorKey", MonitorKey);
        }
    }
}
