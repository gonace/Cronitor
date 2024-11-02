using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests
{
    public class ListPingsRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors/:key/pings";
        public string Key { get; set; }

        public ListPingsRequest(string monitorKey)
        {
            Key = monitorKey;
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
