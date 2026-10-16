using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using Cronitor.Abstractions;

namespace Cronitor.Requests;

public class CloneMonitorRequest : BaseRequest
{
    public override string Endpoint { get; set; } = "monitors/clone";
    public override HttpMethod Method => HttpMethod.Post;

    public CloneMonitorRequest(MonitorKey key, string name = null)
    {
        Content = new StringContent(Serializer.Serialize(new CloneMonitorRequestContent(key, name)), Encoding.UTF8, "application/json");
    }

    private class CloneMonitorRequestContent
    {
        public CloneMonitorRequestContent(MonitorKey key, string name)
        {
            Key = key;
            Name = name;
        }

        [JsonPropertyName("key")]
        public MonitorKey Key { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
