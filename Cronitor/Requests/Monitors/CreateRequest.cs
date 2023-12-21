using Cronitor.Abstractions;
using Cronitor.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Cronitor.Requests.Monitors
{
    public class CreateRequest : BaseRequest
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
