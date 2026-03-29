using Cronitor.Abstractions;
using Cronitor.Serialization;
using System.Net.Http;
using System.Text;
using Cronitor.Models;

namespace Cronitor.Requests
{
    public class CreateNotificationRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "notifications";
        public override HttpMethod Method => HttpMethod.Post;

        public CreateNotificationRequest(Notification template)
        {
            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }

        public CreateNotificationRequest(string name, string key, Notifications notifications)
        {
            var template = new Notification
            {
                Name = name,
                Key = key,
                Notifications = notifications
            };

            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }
    }
}
