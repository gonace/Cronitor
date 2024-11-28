using System.Text.Json;
using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System.Threading.Tasks;

namespace Cronitor.Clients
{
    public interface INotificationsClient
    {
        ListNotificationResponse List();
        Task<ListNotificationResponse> ListAsync();
        Template Get(string name);
        Task<Template> GetAsync(string name);
        Template Create(CreateNotificationRequest request);
        Task<Template> CreateAsync(CreateNotificationRequest request);
        Template Update(UpdateNotificationRequest request);
        Task<Template> UpdateAsync(UpdateNotificationRequest request);
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

        public NotificationsClient(string apiKey, JsonSerializerOptions jsonSerializerOptions)
            : base(Urls.ApiUrl, apiKey, jsonSerializerOptions)
        {
        }

        internal NotificationsClient(HttpClient client)
            : base(client)
        {
        }

        public ListNotificationResponse List() =>
            Task.Run(async () => await ListAsync()).Result;

        public async Task<ListNotificationResponse> ListAsync()
        {
            var request = new ListNotificationRequest();
            var response = await SendAsync<ListNotificationResponse>(request);

            return response;
        }

        public Template Get(string name) =>
            Task.Run(async () => await GetAsync(name)).Result;

        public async Task<Template> GetAsync(string name)
        {
            var request = new GetNotificationRequest(name);

            return await SendAsync<Template>(request);
        }

        public Template Create(CreateNotificationRequest request) =>
            Task.Run(async () => await CreateAsync(request)).Result;

        public async Task<Template> CreateAsync(CreateNotificationRequest request) =>
            await SendAsync<Template>(request);

        public Template Update(UpdateNotificationRequest request) =>
            Task.Run(async () => await UpdateAsync(request)).Result;

        public async Task<Template> UpdateAsync(UpdateNotificationRequest request) =>
            await SendAsync<Template>(request);

        public void Delete(string key) =>
            Task.Run(async () => await DeleteAsync(key))
                .Wait();

        public async Task DeleteAsync(string key)
        {
            var request = new DeleteNotificationRequest(key);

            await SendAsync<Task>(request);
        }
    }
}
