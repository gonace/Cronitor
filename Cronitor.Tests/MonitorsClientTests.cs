using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Builders;
using Cronitor.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class MonitorsClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly MonitorsClient _monitorsClient;

        public MonitorsClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _monitorsClient = new MonitorsClient(_httpClient.Object);
        }

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
            var response = json.Deserialize<ListMonitorResponse>();
            _httpClient.Setup(x => x.SendAsync<ListMonitorResponse>(It.IsAny<ListMonitorRequest>())).Returns(Task.FromResult(response));

            var result = _monitorsClient.List();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            _httpClient.Verify(x => x.SendAsync<ListMonitorResponse>(It.Is<ListMonitorRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListMonitorResponse { Items = new List<Monitor> { Make.Check.Build(), Make.Heartbeat.Build(), Make.Job.Build() } };
            _httpClient.Setup(x => x.SendAsync<ListMonitorResponse>(It.IsAny<ListMonitorRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.ListAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
           _httpClient.Verify(x => x.SendAsync<ListMonitorResponse>(It.Is<ListMonitorRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteGetMethod()
        {
            var response = new Monitor(MonitorKey);
            _httpClient.Setup(x => x.SendAsync<Monitor>(It.IsAny<GetMonitorRequest>())).Returns(Task.FromResult(response));

            var result = _monitorsClient.Get(MonitorKey);

            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);
            _httpClient.Verify(x => x.SendAsync<Monitor>(It.Is<GetMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = new Monitor(MonitorKey);
            _httpClient.Setup(x => x.SendAsync<Monitor>(It.IsAny<GetMonitorRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.GetAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);
            _httpClient.Verify(x => x.SendAsync<Monitor>(It.Is<GetMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
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
                .WithSchedule("every 60 seconds")
                .Build();
            _httpClient.Setup(x => x.SendAsync<CreateMonitorResponse>(It.IsAny<CreateMonitorRequest>())).Returns(Task.FromResult(new CreateMonitorResponse { Monitors = new List<Monitor> { monitor } }));

            var result = _monitorsClient.Create(new CreateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ShouldExecuteCreateAsyncMethod()
        {
            var monitor = Make.Job.Build();
            _httpClient.Setup(x => x.SendAsync<CreateMonitorResponse>(It.IsAny<CreateMonitorRequest>())).Returns(Task.FromResult(new CreateMonitorResponse { Monitors = new List<Monitor> { monitor } }));

            var result = await _monitorsClient.CreateAsync(new CreateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ShouldExecuteUpdateMethod()
        {
            var monitor = Make.Job.Key(MonitorKey).Build();
            var response = new UpdateMonitorResponse { Monitors = new List<Monitor> { monitor } };
            _httpClient.Setup(x => x.SendAsync<UpdateMonitorResponse>(It.IsAny<UpdateMonitorRequest>())).Returns(Task.FromResult(response));

            var result = _monitorsClient.Update(new UpdateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<UpdateMonitorResponse>(It.Is<UpdateMonitorRequest>(c =>
                c.Method == HttpMethod.Put &&
                c.Endpoint == "monitors")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteUpdateAsyncMethod()
        {
            var monitor = Make.Job.Key(MonitorKey).Build();
            var response = new UpdateMonitorResponse { Monitors = new List<Monitor> { monitor } };
            _httpClient.Setup(x => x.SendAsync<UpdateMonitorResponse>(It.IsAny<UpdateMonitorRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.UpdateAsync(new UpdateMonitorRequest(monitor));

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<UpdateMonitorResponse>(It.Is<UpdateMonitorRequest>(c =>
                c.Method == HttpMethod.Put &&
                c.Endpoint == "monitors")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteDeleteMethod()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<DeleteMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            _monitorsClient.Delete(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<DeleteMonitorRequest>(c =>
                c.Method == HttpMethod.Delete &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteDeleteAsyncMethod()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<DeleteMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            await _monitorsClient.DeleteAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<DeleteMonitorRequest>(c =>
                c.Method == HttpMethod.Delete &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecutePauseMethod()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<PauseMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            _monitorsClient.Pause(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<PauseMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pause")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecutePauseAsyncMethod()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<PauseMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            await _monitorsClient.PauseAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<PauseMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pause")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecutePauseMethodWithHours()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<PauseMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            _monitorsClient.Pause(MonitorKey, 24);

            _httpClient.Verify(x => x.SendAsync(It.Is<PauseMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pause/:duration")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecutePauseAsyncMethodWithHours()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<PauseMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            await _monitorsClient.PauseAsync(MonitorKey, 24);

            _httpClient.Verify(x => x.SendAsync(It.Is<PauseMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pause/:duration")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteUnpauseMethod()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<UnpauseMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            _monitorsClient.Unpause(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<UnpauseMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pause/:duration")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteUnpauseAsyncMethod()
        {
            _httpClient.Setup(x => x.SendAsync(It.IsAny<UnpauseMonitorRequest>())).Returns(Task.FromResult(Task.CompletedTask));

            await _monitorsClient.UnpauseAsync(MonitorKey);

            _httpClient.Verify(x => x.SendAsync(It.Is<UnpauseMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pause/:duration")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteActivitiesMethod()
        {
            var activities = new List<Activity> { new Activity() };
            _httpClient.Setup(x => x.SendAsync<IEnumerable<Activity>>(It.IsAny<ListActivitiesRequest>())).Returns(Task.FromResult<IEnumerable<Activity>>(activities));

            var result = _monitorsClient.Activities(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<IEnumerable<Activity>>(It.Is<ListActivitiesRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/activity")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteActivitiesAsyncMethod()
        {
            var activities = new List<Activity> { new Activity() };
            _httpClient.Setup(x => x.SendAsync<IEnumerable<Activity>>(It.IsAny<ListActivitiesRequest>())).Returns(Task.FromResult<IEnumerable<Activity>>(activities));

            var result = await _monitorsClient.ActivitiesAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<IEnumerable<Activity>>(It.Is<ListActivitiesRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/activity")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteAlertsMethod()
        {
            var alerts = new List<Alert> { new Alert() };
            var response = new Dictionary<string, IEnumerable<Alert>> { { MonitorKey, alerts } };
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.IsAny<ListAlertsRequest>())).Returns(Task.FromResult(response));

            var result = _monitorsClient.Alerts(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.Is<ListAlertsRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/alerts")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteAlertsAsyncMethod()
        {
            var alerts = new List<Alert> { new Alert() };
            var response = new Dictionary<string, IEnumerable<Alert>> { { MonitorKey, alerts } };
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.IsAny<ListAlertsRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.AlertsAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.Is<ListAlertsRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/alerts")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecutePingsMethod()
        {
            var pings = new List<Ping> { new Ping() };
            var response = new Dictionary<string, IEnumerable<Ping>> { { MonitorKey, pings } };
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.IsAny<ListPingsRequest>())).Returns(Task.FromResult(response));

            var result = _monitorsClient.Pings(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.Is<ListPingsRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pings")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecutePingsAsyncMethod()
        {
            var pings = new List<Ping> { new Ping() };
            var response = new Dictionary<string, IEnumerable<Ping>> { { MonitorKey, pings } };
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.IsAny<ListPingsRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.PingsAsync(MonitorKey);

            Assert.NotNull(result);
            Assert.Single(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.Is<ListPingsRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key/pings")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateAsyncShouldReturnNullWhenResponseIsNull()
        {
            _httpClient.Setup(x => x.SendAsync<CreateMonitorResponse>(It.IsAny<CreateMonitorRequest>())).Returns(Task.FromResult<CreateMonitorResponse>(null));

            var monitor = Make.Job.Build();
            var result = await _monitorsClient.CreateAsync(new CreateMonitorRequest(monitor));

            Assert.Null(result);
            _httpClient.Verify(x => x.SendAsync<CreateMonitorResponse>(It.IsAny<CreateMonitorRequest>()), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AlertsAsyncShouldReturnNullWhenResponseIsNull()
        {
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.IsAny<ListAlertsRequest>())).Returns(Task.FromResult<Dictionary<string, IEnumerable<Alert>>>(null));

            var result = await _monitorsClient.AlertsAsync(MonitorKey);

            Assert.Null(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.IsAny<ListAlertsRequest>()), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AlertsAsyncShouldReturnNullWhenDictionaryIsEmpty()
        {
            var response = new Dictionary<string, IEnumerable<Alert>>();
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.IsAny<ListAlertsRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.AlertsAsync(MonitorKey);

            Assert.Null(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Alert>>>(It.IsAny<ListAlertsRequest>()), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task PingsAsyncShouldReturnNullWhenResponseIsNull()
        {
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.IsAny<ListPingsRequest>())).Returns(Task.FromResult<Dictionary<string, IEnumerable<Ping>>>(null));

            var result = await _monitorsClient.PingsAsync(MonitorKey);

            Assert.Null(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.IsAny<ListPingsRequest>()), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task PingsAsyncShouldReturnNullWhenDictionaryIsEmpty()
        {
            var response = new Dictionary<string, IEnumerable<Ping>>();
            _httpClient.Setup(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.IsAny<ListPingsRequest>())).Returns(Task.FromResult(response));

            var result = await _monitorsClient.PingsAsync(MonitorKey);

            Assert.Null(result);
            _httpClient.Verify(x => x.SendAsync<Dictionary<string, IEnumerable<Ping>>>(It.IsAny<ListPingsRequest>()), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }
    }
}
