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

        public CreateRequest(string name, string key, Models.Notifications notifications)
        {
            var template = new Models.Template
            {
                Name = name,
                Key = key,
                Notifications = notifications
            };

            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }
    }
}
