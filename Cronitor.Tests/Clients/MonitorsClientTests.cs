using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Models.Monitors;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests.Clients
{
    public class MonitorsClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly MonitorsClient _client;

        public MonitorsClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _client = new MonitorsClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteListMethod()
        {
            var response = new ListMonitorResponse { Items = new List<Monitor> { Check, Heartbeat, Job } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListMonitorResponse>(It.IsAny<ListMonitorRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _client.List();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<ListMonitorResponse>(It.Is<ListMonitorRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteListAsyncMethod()
        {
            var response = new ListMonitorResponse { Items = new List<Monitor> { Check, Heartbeat, Job } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<ListMonitorResponse>(It.IsAny<ListMonitorRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _client.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
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

            // Setup
            _httpClient.Setup(x => x.SendAsync<Monitor>(It.IsAny<GetMonitorRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _client.Get(MonitorKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Monitor>(It.Is<GetMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = new Monitor(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync<Monitor>(It.IsAny<GetMonitorRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _client.GetAsync(MonitorKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Monitor>(It.Is<GetMonitorRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldExecuteCreateMethod()
        {
            var assertions = new List<string>
            {
                "metric.duration < 30s",
                "metric.error_count < 5"
            };
            var notify = new List<string>
            {
                "developers",
                "administrators"
            };
            const string schedule = "every 60 seconds";
            const string timezone = "Europe/Stockholm";

            var monitor = new Job(MonitorKey)
            {
                Assertions = assertions,
                Notify = notify,
                Schedule = schedule,
                Timezone = timezone
            };

            // Setup
            _httpClient.Setup(x => x.SendAsync<CreateMonitorResponse>(It.IsAny<CreateMonitorRequest>())).Returns(Task.FromResult(new CreateMonitorResponse { Monitors = new List<Monitor> { (Monitor)monitor } }));

            // Run
            var result = _client.Create(new CreateMonitorRequest(monitor));

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ShouldExecuteCreateAsyncMethod()
        {
            var monitor = Job;

            // Setup
            _httpClient.Setup(x => x.SendAsync<CreateMonitorResponse>(It.IsAny<CreateMonitorRequest>())).Returns(Task.FromResult(new CreateMonitorResponse { Monitors = new List<Monitor> { (Monitor)monitor } }));

            // Run
            var result = await _client.CreateAsync(new CreateMonitorRequest(monitor));

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
