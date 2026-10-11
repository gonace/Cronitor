using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Builders;
using Cronitor.Tests.Helpers;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class NotificationsClientTests : BaseTest
    {
        private const string NotificationsUri = "https://cronitor.io/api/notifications";

        [Fact]
        public void ShouldConstructWithNoParameters()
        {
            var client = new NotificationsClient();

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldConstructWithApiKey()
        {
            var client = new NotificationsClient(ApiKey);

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldDisposeNotificationsClient()
        {
            using (var client = new NotificationsClient(ApiKey))
            {
                Assert.NotNull(client);
            }
            // Test passes if no exception is thrown during disposal
        }

        [Fact]
        public void ShouldExecuteListMethod()
        {
            var response = new ListNotificationResponse { Items = new List<Notification> { Make.Template.Build() } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var notificationsClient = CreateClient(handler);

            var result = notificationsClient.List();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            AssertRequest.Sent(handler, HttpMethod.Get, NotificationsUri + "?page=1");
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListNotificationResponse { Items = new List<Notification> { Make.Template.Build() } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var notificationsClient = CreateClient(handler);

            var result = await notificationsClient.ListAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            AssertRequest.Sent(handler, HttpMethod.Get, NotificationsUri + "?page=1");
        }

        [Fact]
        public void ShouldExecuteGetMethod()
        {
            var response = Make.Template.Build();
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var notificationsClient = CreateClient(handler);

            var result = notificationsClient.Get(TemplateKey);

            Assert.NotNull(result);
            AssertRequest.Sent(handler, HttpMethod.Get, NotificationsUri + "/" + TemplateKey);
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = Make.Template.Build();
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var notificationsClient = CreateClient(handler);

            var result = await notificationsClient.GetAsync(TemplateKey);

            Assert.NotNull(result);
            AssertRequest.Sent(handler, HttpMethod.Get, NotificationsUri + "/" + TemplateKey);
        }

        [Fact]
        public void ShouldExecuteCreateMethod()
        {
            var notifications = new Notifications
            {
                Emails = new[] { "jane.doe@cronitor.io", "john.doe@cronitor.io" }
            };
            var request = new CreateNotificationRequest("Notification", TemplateKey, notifications);
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(Make.Template.Build()));
            var notificationsClient = CreateClient(handler);

            var result = notificationsClient.Create(request);

            Assert.NotNull(result);
            AssertRequest.Sent(handler, HttpMethod.Post, NotificationsUri);
        }

        [Fact]
        public async Task ShouldExecuteCreateAsyncMethod()
        {
            var notifications = new Notifications
            {
                Emails = new[] { "jane.doe@cronitor.io", "john.doe@cronitor.io" }
            };
            var request = new CreateNotificationRequest("Notification", TemplateKey, notifications);
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(Make.Template.Build()));
            var notificationsClient = CreateClient(handler);

            var result = await notificationsClient.CreateAsync(request);

            Assert.NotNull(result);
            AssertRequest.Sent(handler, HttpMethod.Post, NotificationsUri);
        }

        [Fact]
        public void ShouldExecuteUpdateMethod()
        {
            var response = Make.Template.Build();
            var request = new UpdateNotificationRequest(TemplateKey, response);
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var notificationsClient = CreateClient(handler);

            var result = notificationsClient.Update(request);

            Assert.NotNull(result);
            AssertRequest.Sent(handler, HttpMethod.Put, NotificationsUri + "/" + TemplateKey);
        }

        [Fact]
        public async Task ShouldExecuteUpdateAsyncMethod()
        {
            var response = Make.Template.Build();
            var request = new UpdateNotificationRequest(TemplateKey, response);
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var notificationsClient = CreateClient(handler);

            var result = await notificationsClient.UpdateAsync(request);

            Assert.NotNull(result);
            AssertRequest.Sent(handler, HttpMethod.Put, NotificationsUri + "/" + TemplateKey);
        }

        [Fact]
        public void ShouldExecuteDeleteMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var notificationsClient = CreateClient(handler);

            notificationsClient.Delete(TemplateKey);

            AssertRequest.Sent(handler, HttpMethod.Delete, NotificationsUri + "/" + TemplateKey);
        }

        [Fact]
        public async Task ShouldExecuteDeleteAsyncMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var notificationsClient = CreateClient(handler);

            await notificationsClient.DeleteAsync(TemplateKey);

            AssertRequest.Sent(handler, HttpMethod.Delete, NotificationsUri + "/" + TemplateKey);
        }

        private static NotificationsClient CreateClient(FakeHttpMessageHandler handler)
        {
            return new NotificationsClient(new Internals.HttpClient(Urls.DefaultApiUrl, ApiKey, handler));
        }
    }
}
