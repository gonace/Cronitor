using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests.Notifications;
using Cronitor.Responses.Notifications;
using System.Threading.Tasks;

namespace Cronitor.Clients
{
    public interface INotificationsClient
    {
        ListResponse List();
        Task<ListResponse> ListAsync();
        Template Get(string name);
        Task<Template> GetAsync(string name);
        Template Create(CreateRequest request);
        Task<Template> CreateAsync(CreateRequest request);
        Template Update(UpdateRequest request);
        Task<Template> UpdateAsync(UpdateRequest request);
        void Delete(string key);
        Task DeleteAsync(string key);
    }

    public class NotificationsClient : BaseClient<NotificationsClient>, INotificationsClient
    {
        public NotificationsClient()
            : base(Urls.ApiUrl)
        {
        }

        public NotificationsClient(string apiKey)
            : base(Urls.ApiUrl, apiKey)
        {
        }

        public NotificationsClient(string apiKey, bool useHttps)
            : base(Urls.ApiUrl, apiKey, useHttps)
        {
        }

        internal NotificationsClient(HttpClient client)
            : base(client)
        {
        }


        public ListResponse List() =>
            Task.Run(async () => await ListAsync()).Result;

        public async Task<ListResponse> ListAsync()
        {
            var request = new ListRequest();
            var response = await SendAsync<ListResponse>(request);

            return response;
        }

        public Template Get(string name) =>
            Task.Run(async () => await GetAsync(name)).Result;

        public async Task<Template> GetAsync(string name)
        {
            var request = new GetRequest(name);

            return await SendAsync<Template>(request);
        }

        public Template Create(CreateRequest request) =>
            Task.Run(async () => await CreateAsync(request)).Result;

        public async Task<Template> CreateAsync(CreateRequest request) =>
            await SendAsync<Template>(request);

        public Template Update(UpdateRequest request) =>
            Task.Run(async () => await UpdateAsync(request)).Result;

        public async Task<Template> UpdateAsync(UpdateRequest request) =>
            await SendAsync<Template>(request);

        public void Delete(string key) =>
            Task.Run(async () => await DeleteAsync(key))
                .Wait();

        public async Task DeleteAsync(string key)
        {
            var request = new DeleteRequest(key);

            await SendAsync<Task>(request);
        }
    }
}
