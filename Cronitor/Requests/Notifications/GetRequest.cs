using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Notifications
{
    public class GetRequest : Request
    {
        public override string Endpoint { get; set; } = "templates/:key";
        public string Key { get; set; }

        public GetRequest(string key)
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
