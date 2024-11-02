using Cronitor.Abstractions;
using Cronitor.Serialization;
using System.Net.Http;
using System.Text;

namespace Cronitor.Requests
{
    public class CreateNotificationRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "templates";
        public override HttpMethod Method => HttpMethod.Post;

        public CreateNotificationRequest(Models.Template template)
        {
            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }

        public CreateNotificationRequest(string name, string key, Models.Notifications notifications)
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
