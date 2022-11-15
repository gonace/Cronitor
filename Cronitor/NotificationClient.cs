using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Requests.Notifications;
using System.Threading.Tasks;

namespace Cronitor
{
    public class NotificationClient : BaseClient
    {
        public NotificationClient()
            : base(Urls.ApiUrl)
        {
        }

        public NotificationClient(string apiKey)
            : base(Urls.ApiUrl, apiKey)
        {
        }

        public NotificationClient(string apiKey, bool useHttps)
            : base(Urls.ApiUrl, apiKey, useHttps)
        {
        }

        internal NotificationClient(HttpClient client)
            : base(client)
        {
        }


        public Pageable<Template> Find() =>
            Task.Run(async () => await FindAsync()).Result;

        public async Task<Pageable<Template>> FindAsync()
        {
            var request = new FindRequest();
            var response = await SendAsync<Pageable<Template>>(request);

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
