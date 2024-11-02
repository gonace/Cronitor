using Cronitor.Abstractions;
using Cronitor.Models;
using Cronitor.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Cronitor.Requests
{
    public class CreateIssueRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues";
        public override HttpMethod Method => HttpMethod.Post;

        public CreateIssueRequest(Issue issue)
        {
            Content = new StringContent(Serializer.Serialize(new { monitors = new List<Issue> { issue } }), Encoding.UTF8, "application/json");
        }

        public CreateIssueRequest(IEnumerable<Issue> issues)
        {
            Content = new StringContent(Serializer.Serialize(new { monitors = issues }), Encoding.UTF8, "application/json");
        }
    }
}
