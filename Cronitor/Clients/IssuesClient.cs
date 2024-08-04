using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests.Issues;
using Cronitor.Responses.Issues;
using System.Threading.Tasks;

namespace Cronitor.Clients
{
    public interface IIssuesClient
    {
        ListResponse List(int page = 1);
        Task<ListResponse> ListAsync(int page = 1);
        Issue Get(string key);
        Task<Issue> GetAsync(string key);
        Issue Create(CreateRequest request);
        Task<Issue> CreateAsync(CreateRequest request);
        Issue Update(UpdateRequest request);
        Task<Issue> UpdateAsync(UpdateRequest request);
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

        public IssuesClient(string apiKey, bool useHttps)
            : base(Urls.ApiUrl, apiKey, useHttps)
        {
        }

        internal IssuesClient(HttpClient client)
            : base(client)
        {
        }

        public ListResponse List(int page = 1) =>
            Task.Run(async () => await ListAsync(page)).Result;

        public async Task<ListResponse> ListAsync(int page = 1)
        {
            var request = new ListRequest
            {
                Page = page
            };

            return await SendAsync<ListResponse>(request);
        }

        public Issue Get(string key) =>
            Task.Run(async () => await GetAsync(key)).Result;

        public async Task<Issue> GetAsync(string key)
        {
            var request = new GetRequest(key);

            return await SendAsync<Issue>(request);
        }

        public Issue Create(CreateRequest request) =>
            Task.Run(async () => await CreateAsync(request)).Result;

        public async Task<Issue> CreateAsync(CreateRequest request)
        {
            return await SendAsync<Issue>(request);
        }

        public Issue Update(UpdateRequest request) =>
            Task.Run(async () => await UpdateAsync(request)).Result;

        public async Task<Issue> UpdateAsync(UpdateRequest request)
        {
            return await SendAsync<Issue>(request);
        }
    }
}
