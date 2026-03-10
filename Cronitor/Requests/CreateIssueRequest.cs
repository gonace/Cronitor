using Cronitor.Abstractions;
using Cronitor.Models;
using Cronitor.Serialization;
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
            Content = new StringContent(Serializer.Serialize(issue), Encoding.UTF8, "application/json");
        }
    }
}
