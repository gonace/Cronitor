using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Issues
{
    public class GetRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues/:key";
        public string Key { get; set; }

        public GetRequest(string issueKey)
        {
            Key = issueKey;
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
