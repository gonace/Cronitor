using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests
{
    public class GetNotificationRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "templates/:key";
        public string Key { get; set; }

        public GetNotificationRequest(string key)
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
