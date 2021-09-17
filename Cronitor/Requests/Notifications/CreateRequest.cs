using System.Net.Http;
using System.Text;

namespace Cronitor.Requests.Notifications
{
    public class CreateRequest : Request
    {
        public override string Endpoint { get; set; } = "templates";
        public override HttpMethod Method => HttpMethod.Post;

        public CreateRequest(Models.Template template)
        {
            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }

        //public CreateRequest(IEnumerable<Models.Monitor> monitors)
        //{
        //    Content = new StringContent(Serializer.Serialize(new { monitors }), Encoding.UTF8, "application/json");
        //}
    }
}
