using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cronitor.Serialization;

namespace Cronitor.Requests.Monitor
{
    public class CreateRequest : Request
    {
        public override string Endpoint { get; set; } = "monitors";
        public override HttpMethod Method => HttpMethod.Put;

        public CreateRequest(Models.Monitor monitor)
        {
            Content = new StringContent(Serializer.Serialize(new { monitors = new List<Models.Monitor> { monitor } }), Encoding.UTF8, "application/json");
        }

        public CreateRequest(IEnumerable<Models.Monitor> monitors)
        {
            Content = new StringContent(Serializer.Serialize(new { monitors }), Encoding.UTF8, "application/json");
        }
    }
}
