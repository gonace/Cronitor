using Cronitor.Abstractions;
using Cronitor.Extensions;
using Cronitor.Models;
using Cronitor.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Cronitor.Requests
{
    public class UpdateIssueRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues/:key";
        public string Key { get; set; }
        public override HttpMethod Method => HttpMethod.Put;

        public UpdateIssueRequest(string issueKey, Issue issue)
        {
            Key = issueKey;
            Content = new StringContent(Serializer.Serialize(new { issue }), Encoding.UTF8, "application/json");
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
