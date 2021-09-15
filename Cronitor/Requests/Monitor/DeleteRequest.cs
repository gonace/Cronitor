using System.Net.Http;

namespace Cronitor.Requests.Monitor
{
    public class DeleteRequest : Request
    {
        public override string Endpoint { get; set; } = "monitors/:monitorKey";
        public override HttpMethod Method => HttpMethod.Delete;
        public string MonitorKey { get; set; }

        public DeleteRequest(string monitorKey)
        {
            MonitorKey = monitorKey;
        }

        public override string ToUrl()
        {
            return Endpoint.Replace(":monitorKey", MonitorKey);
        }
    }
}
