using System;
using System.Collections.Generic;
using System.Net.Http;
using Cronitor.Abstractions;
using Cronitor.Extensions;

namespace Cronitor.Requests
{
    public class DeleteIssueRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues/:key";
        public override HttpMethod Method => HttpMethod.Delete;
        public string Key { get; set; }

        public DeleteIssueRequest(string issueKey)
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