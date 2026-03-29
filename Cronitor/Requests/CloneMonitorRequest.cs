using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using Cronitor.Abstractions;
using Cronitor.Serialization;

namespace Cronitor.Requests
{
    public class CloneMonitorRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors/clone";
        public override HttpMethod Method => HttpMethod.Post;

        public CloneMonitorRequest(string key, string name = null)
        {
            Content = new StringContent(Serializer.Serialize(new CloneMonitorRequestContent(key, name)), Encoding.UTF8, "application/json");
        }

        private class CloneMonitorRequestContent
        {
            public CloneMonitorRequestContent(string key, string name)
            {
                Key = key;
                Name = name;
            }

            [JsonPropertyName("key")]
            public string Key { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}