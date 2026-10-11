using Cronitor.Clients;
using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Scheduling;
using Cronitor.Serialization;
using Cronitor.Tests.Builders;
using Cronitor.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class MonitorsClientTests : BaseTest
    {
        private const string MonitorsUri = "https://cronitor.io/api/monitors";
        private const string MonitorUri = MonitorsUri + "/" + MonitorKey;

        [Fact]
        public void ShouldConstructWithNoParameters()
        {
            var client = new MonitorsClient();

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldConstructWithApiKey()
        {
            var client = new MonitorsClient(ApiKey);

            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldDisposeMonitorsClient()
        {
            using (var client = new MonitorsClient(ApiKey))
            {
                Assert.NotNull(client);
            }
            // Test passes if no exception is thrown during disposal
        }

        [Theory]
        [JsonData("ListMonitors.json")]
        public void ShouldExecuteListMethod(string json)
        {
            var handler = new FakeHttpMessageHandler(body: json);
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.List();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "?page=1");
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListMonitorResponse { Items = new List<Monitor> { Make.Check.Build(), Make.Heartbeat.Build(), Make.Job.Build() } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.ListAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "?page=1");
        }

        [Fact]
        public void ShouldExecuteGetMethod()
        {
            var response = new Monitor(MonitorKey);
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.Get(MonitorKey);

            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri);
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = new Monitor(MonitorKey);
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.GetAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri);
        }

        [Fact]
        public void ShouldExecuteCreateMethod()
        {
            var notify = new List<string>
            {
                "developers",
                "administrators"
            };
            var monitor = Make.Job
                .Notify(notify)
                .Schedule(Schedule.Every(60).Seconds)
                .Build();
            var response = new CreateMonitorResponse { Monitors = new List<Monitor> { monitor } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.Create(new CreateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
        }

        [Fact]
        public async Task ShouldExecuteCreateAsyncMethod()
        {
            var monitor = Make.Job.Build();
            var response = new CreateMonitorResponse { Monitors = new List<Monitor> { monitor } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.CreateAsync(new CreateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
        }

        [Fact]
        public void ShouldExecuteUpdateMethod()
        {
            var monitor = Make.Job.Key(MonitorKey).Build();
            var response = new UpdateMonitorResponse { Monitors = new List<Monitor> { monitor } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.Update(new UpdateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
        }

        [Fact]
        public async Task ShouldExecuteUpdateAsyncMethod()
        {
            var monitor = Make.Job.Key(MonitorKey).Build();
            var response = new UpdateMonitorResponse { Monitors = new List<Monitor> { monitor } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.UpdateAsync(new UpdateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
        }

        [Fact]
        public void ShouldExecuteDeleteMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            monitorsClient.Delete(MonitorKey);

            AssertRequest.Sent(handler, HttpMethod.Delete, MonitorUri);
        }

        [Fact]
        public async Task ShouldExecuteDeleteAsyncMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            await monitorsClient.DeleteAsync(MonitorKey);

            AssertRequest.Sent(handler, HttpMethod.Delete, MonitorUri);
        }

        [Fact]
        public void ShouldExecutePauseMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            monitorsClient.Pause(MonitorKey);

            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause");
        }

        [Fact]
        public async Task ShouldExecutePauseAsyncMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            await monitorsClient.PauseAsync(MonitorKey);

            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause");
        }

        [Fact]
        public void ShouldExecutePauseMethodWithHours()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            monitorsClient.Pause(MonitorKey, 24);

            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/24");
        }

        [Fact]
        public async Task ShouldExecutePauseAsyncMethodWithHours()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            await monitorsClient.PauseAsync(MonitorKey, 24);

            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/24");
        }

        [Fact]
        public void ShouldExecuteUnpauseMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            monitorsClient.Unpause(MonitorKey);

            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/0");
        }

        [Fact]
        public async Task ShouldExecuteUnpauseAsyncMethod()
        {
            var handler = new FakeHttpMessageHandler();
            var monitorsClient = CreateClient(handler);

            await monitorsClient.UnpauseAsync(MonitorKey);

            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/0");
        }

        [Fact]
        public void ShouldExecuteActivitiesMethod()
        {
            var activities = new List<Activity> { new Activity() };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(activities));
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.Activities(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/activity");
        }

        [Fact]
        public async Task ShouldExecuteActivitiesAsyncMethod()
        {
            var activities = new List<Activity> { new Activity() };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(activities));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.ActivitiesAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/activity");
        }

        [Fact]
        public void ShouldExecuteAlertsMethod()
        {
            var alerts = new List<Alert> { new Alert() };
            var response = new Dictionary<string, IEnumerable<Alert>> { { MonitorKey, alerts } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.Alerts(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
        }

        [Fact]
        public async Task ShouldExecuteAlertsAsyncMethod()
        {
            var alerts = new List<Alert> { new Alert() };
            var response = new Dictionary<string, IEnumerable<Alert>> { { MonitorKey, alerts } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.AlertsAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
        }

        [Fact]
        public void ShouldExecutePingsMethod()
        {
            var pings = new List<Ping> { new Ping() };
            var response = new Dictionary<string, IEnumerable<Ping>> { { MonitorKey, pings } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = monitorsClient.Pings(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
        }

        [Fact]
        public async Task ShouldExecutePingsAsyncMethod()
        {
            var pings = new List<Ping> { new Ping() };
            var response = new Dictionary<string, IEnumerable<Ping>> { { MonitorKey, pings } };
            var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.PingsAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
        }

        [Fact]
        public async Task CreateAsyncShouldReturnNullWhenResponseIsNull()
        {
            var handler = new FakeHttpMessageHandler(body: "null");
            var monitorsClient = CreateClient(handler);

            var monitor = Make.Job.Build();
            var result = await monitorsClient.CreateAsync(new CreateMonitorRequest(monitor));

            Assert.Null(result);
            AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
        }

        [Fact]
        public async Task AlertsAsyncShouldReturnNullWhenResponseIsNull()
        {
            var handler = new FakeHttpMessageHandler(body: "null");
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.AlertsAsync(MonitorKey);

            Assert.Null(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
        }

        [Fact]
        public async Task AlertsAsyncShouldReturnNullWhenDictionaryIsEmpty()
        {
            var handler = new FakeHttpMessageHandler(body: "{}");
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.AlertsAsync(MonitorKey);

            Assert.Null(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
        }

        [Fact]
        public async Task PingsAsyncShouldReturnNullWhenResponseIsNull()
        {
            var handler = new FakeHttpMessageHandler(body: "null");
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.PingsAsync(MonitorKey);

            Assert.Null(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
        }

        [Fact]
        public async Task PingsAsyncShouldReturnNullWhenDictionaryIsEmpty()
        {
            var handler = new FakeHttpMessageHandler(body: "{}");
            var monitorsClient = CreateClient(handler);

            var result = await monitorsClient.PingsAsync(MonitorKey);

            Assert.Null(result);
            AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
        }

        [Fact]
        public async Task GetAsyncShouldThrowApiExceptionWhenMonitorIsNotFound()
        {
            var handler = new FakeHttpMessageHandler(HttpStatusCode.NotFound, "{\"detail\":\"Monitor not found\"}");
            var monitorsClient = CreateClient(handler);

            var exception = await Assert.ThrowsAsync<ApiException>(() => monitorsClient.GetAsync(MonitorKey));

            Assert.Equal("Monitor not found (NotFound)", exception.Message);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowApiExceptionWhenUnauthorized()
        {
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Unauthorized, "{\"detail\":\"Invalid API key\"}");
            var monitorsClient = CreateClient(handler);

            var exception = await Assert.ThrowsAsync<ApiException>(() => monitorsClient.DeleteAsync(MonitorKey));

            Assert.Equal("Invalid API key (Unauthorized)", exception.Message);
        }

        private static MonitorsClient CreateClient(FakeHttpMessageHandler handler)
        {
            return new MonitorsClient(new Internals.HttpClient(Urls.DefaultApiUrl, ApiKey, handler));
        }
    }
}
