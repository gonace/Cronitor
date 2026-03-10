using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System.Threading.Tasks;
using Cronitor.Extensions;

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
            : base(Urls.DefaultApiUrl)
        {
        }

        public IssuesClient(string apiKey)
            : base(Urls.DefaultApiUrl, apiKey)
        {
        }

        internal IssuesClient(HttpClient client)
            : base(client)
        {
        }

        public ListIssueResponse List(int page = 1) =>
            ListAsync(page).GetAwaiter().GetResult();

        public async Task<ListIssueResponse> ListAsync(int page = 1)
        {
            var request = new ListIssueRequest
            {
                Page = page
            };

            return await SendAsync<ListIssueResponse>(request);
        }

        public Issue Get(string key) =>
            GetAsync(key).GetAwaiter().GetResult();

        public async Task<Issue> GetAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new GetIssueRequest(key);

            return await SendAsync<Issue>(request);
        }

        public Issue Create(CreateIssueRequest request) =>
            CreateAsync(request).GetAwaiter().GetResult();

        public async Task<Issue> CreateAsync(CreateIssueRequest request)
        {
            return await SendAsync<Issue>(request);
        }

        public Issue Update(UpdateIssueRequest request) =>
            UpdateAsync(request).GetAwaiter().GetResult();

        public async Task<Issue> UpdateAsync(UpdateIssueRequest request)
        {
            return await SendAsync<Issue>(request);
        }
    }
}
