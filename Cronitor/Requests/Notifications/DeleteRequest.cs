using Cronitor.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Cronitor.Requests.Notifications
{
    public class DeleteRequest : Request
    {
        public override string Endpoint { get; set; } = "templates/:key";
        public override HttpMethod Method => HttpMethod.Delete;
        public string Key { get; set; }

        public DeleteRequest(string key)
        {
            Key = key;
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
