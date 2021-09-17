using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cronitor.Extensions;

namespace Cronitor.Requests.Notifications
{
    public class UpdateRequest : Request
    {
        public override string Endpoint { get; set; } = "templates/:key";
        public override HttpMethod Method => HttpMethod.Put;
        public string Key { get; set; }

        public UpdateRequest(string key, Models.Template template)
        {
            Key = key;
            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", Key }
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
