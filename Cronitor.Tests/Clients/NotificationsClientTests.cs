﻿using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Tests.Builders;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class NotificationsClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly NotificationsClient _notificationsClient;

        public NotificationsClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _notificationsClient = new NotificationsClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteListMethod()
        {
            var response = new ListNotificationResponse { Items = new List<Template> { Make.Template.Build() } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListNotificationResponse>(It.IsAny<ListNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _notificationsClient.List();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListNotificationResponse>(It.Is<ListNotificationRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListNotificationResponse { Items = new List<Template> { Make.Template.Build() } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListNotificationResponse>(It.IsAny<ListNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _notificationsClient.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListNotificationResponse>(It.Is<ListNotificationRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteGetMethod()
        {
            var response = Make.Template.Build();

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<GetNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _notificationsClient.Get(TemplateKey);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<GetNotificationRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = Make.Template.Build();

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<GetNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _notificationsClient.GetAsync(TemplateKey);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<GetNotificationRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteCreateMethod()
        {
            var notifications = new Notifications
            {
                Emails = new[] { "jane.doe@cronitor.io", "john.doe@cronitor.io" }
            };
            var request = new CreateNotificationRequest("Notification", TemplateKey, notifications);
            var response = Make.Template.Build();

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<CreateNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _notificationsClient.Create(request);

            // Assert
            Assert.NotNull(result);
            //Assert.Equivalent(notifications, result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<CreateNotificationRequest>(c =>
                c.Method == HttpMethod.Post &&
                c.Endpoint == "templates")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteCreateAsyncMethod()
        {
            var notifications = new Notifications
            {
                Emails = new[] { "jane.doe@cronitor.io", "john.doe@cronitor.io" }
            };
            var request = new CreateNotificationRequest("Notification", TemplateKey, notifications);
            var response = Make.Template.Build();

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<CreateNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _notificationsClient.CreateAsync(request);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<CreateNotificationRequest>(c =>
                c.Method == HttpMethod.Post &&
                c.Endpoint == "templates")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteUpdateMethod()
        {
            var response = Make.Template.Build();
            var request = new UpdateNotificationRequest(TemplateKey, response);

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<UpdateNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _notificationsClient.Update(request);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<UpdateNotificationRequest>(c =>
                c.Method == HttpMethod.Put &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteUpdateAsyncMethod()
        {
            var response = Make.Template.Build();
            var request = new UpdateNotificationRequest(TemplateKey, response);

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<UpdateNotificationRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _notificationsClient.UpdateAsync(request);

            // Assert
            Assert.NotNull(result);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Template>(It.Is<UpdateNotificationRequest>(c =>
                c.Method == HttpMethod.Put &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteDeleteMethod()
        {

            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<DeleteNotificationRequest>()));

            // Run
            _notificationsClient.Delete(TemplateKey);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Task>(It.Is<DeleteNotificationRequest>(c =>
                c.Method == HttpMethod.Delete &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteDeleteAsyncMethod()
        {
            // Setup
            _httpClient.Setup(x => x.SendAsync<Template>(It.IsAny<DeleteNotificationRequest>()));

            // Run
            await _notificationsClient.DeleteAsync(TemplateKey);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Task>(It.Is<DeleteNotificationRequest>(c =>
                c.Method == HttpMethod.Delete &&
                c.Endpoint == "templates/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}
