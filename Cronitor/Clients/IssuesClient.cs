using System.Text.Json;
using System.Text.Json.Serialization;
using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System.Threading.Tasks;

namespace Cronitor.Clients
{
    public interface IIssuesClient
    {
        ListIssueResponse List(int page = 1);
        Task<ListIssueResponse> ListAsync(int page = 1);
        Issue Get(string key);
        Task<Issue> GetAsync(string key);
        Issue Create(CreateIssueRequest request);
        Task<Issue> CreateAsync(CreateIssueRequest request);
        Issue Update(UpdateIssueRequest request);
        Task<Issue> UpdateAsync(UpdateIssueRequest request);
    }

    public class IssuesClient : BaseClient<IssuesClient>, IIssuesClient
    {
        public IssuesClient()
            : base(Urls.ApiUrl)
        {
        }

        public IssuesClient(string apiKey)
            : base(Urls.ApiUrl, apiKey)
        {
        }

        public IssuesClient(string apiKey, JsonSerializerOptions jsonSerializerOptions)
            : base(Urls.ApiUrl, apiKey, jsonSerializerOptions)
        {
        }

        internal IssuesClient(HttpClient client)
            : base(client)
        {
        }

        public ListIssueResponse List(int page = 1) =>
            Task.Run(async () => await ListAsync(page)).Result;

        public async Task<ListIssueResponse> ListAsync(int page = 1)
        {
            var request = new ListIssueRequest
            {
                Page = page
            };

            return await SendAsync<ListIssueResponse>(request);
        }

        public Issue Get(string key) =>
            Task.Run(async () => await GetAsync(key)).Result;

        public async Task<Issue> GetAsync(string key)
        {
            var request = new GetIssueRequest(key);

            return await SendAsync<Issue>(request);
        }

        public Issue Create(CreateIssueRequest request) =>
            Task.Run(async () => await CreateAsync(request)).Result;

        public async Task<Issue> CreateAsync(CreateIssueRequest request)
        {
            return await SendAsync<Issue>(request);
        }

        public Issue Update(UpdateIssueRequest request) =>
            Task.Run(async () => await UpdateAsync(request)).Result;

        public async Task<Issue> UpdateAsync(UpdateIssueRequest request)
        {
            return await SendAsync<Issue>(request);
        }
    }
}
