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
            : base(Urls.DefaultApiUrl)
        {
        }

        public NotificationsClient(string apiKey)
            : base(Urls.DefaultApiUrl, apiKey)
        {
        }

        internal NotificationsClient(HttpClient client)
            : base(client)
        {
        }

        public ListNotificationResponse List() =>
            ListAsync().GetAwaiter().GetResult();

        public async Task<ListNotificationResponse> ListAsync()
        {
            var request = new ListNotificationRequest();
            var response = await SendAsync<ListNotificationResponse>(request);

            return response;
        }

        public Template Get(string name) =>
            GetAsync(name).GetAwaiter().GetResult();

        public async Task<Template> GetAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new GetNotificationRequest(key);

            return await SendAsync<Template>(request);
        }

        public Template Create(CreateNotificationRequest request) =>
            CreateAsync(request).GetAwaiter().GetResult();

        public async Task<Template> CreateAsync(CreateNotificationRequest request) =>
            await SendAsync<Template>(request);

        public Template Update(UpdateNotificationRequest request) =>
            UpdateAsync(request).GetAwaiter().GetResult();

        public async Task<Template> UpdateAsync(UpdateNotificationRequest request) =>
            await SendAsync<Template>(request);

        public void Delete(string key) =>
            DeleteAsync(key).GetAwaiter().GetResult();

        public async Task DeleteAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new DeleteNotificationRequest(key);

            await SendAsync(request);
        }
    }
}
