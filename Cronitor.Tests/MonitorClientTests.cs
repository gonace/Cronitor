using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Models.Monitors;
using Cronitor.Requests;
using Cronitor.Requests.Monitor;
using Cronitor.Responses.Monitor;
using Cronitor.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class MonitorClientTests : BaseTest
    {
        private readonly Mock<Internals.HttpClient> _httpClient;
        private readonly MonitorClient _client;

        public MonitorClientTests()
        {
            _httpClient = new Mock<Internals.HttpClient>();
            _client = new MonitorClient(_httpClient.Object);
        }

        [Fact]
        public void ShouldExecuteFindMethod()
        {
            var response = new Pageable<Monitor> { Monitors = new List<Monitor> { Check, Heartbeat, Job } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<Pageable<Monitor>>(It.IsAny<FindRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _client.Find();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Result);
            Assert.Equal(3, result.Result.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Pageable<Monitor>>(It.Is<FindRequest>(c =>
                c.Page == 1 &&
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteFindAsyncMethod()
        {
            var response = new Pageable<Monitor> { Monitors = new List<Monitor> { Check, Heartbeat, Job } };

            // Setup
            _httpClient.Setup(x => x.SendAsync<Pageable<Monitor>>(It.IsAny<FindRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _client.FindAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Result);
            Assert.Equal(3, result.Result.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Pageable<Monitor>>(It.Is<FindRequest>(c =>
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
            _httpClient.Setup(x => x.SendAsync<Monitor>(It.IsAny<GetRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = _client.Get(MonitorKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Monitor>(It.Is<GetRequest>(c =>
                c.Method == HttpMethod.Get &&
                c.Endpoint == "monitors/:key")), Times.Once);
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExecuteGetAsyncMethod()
        {
            var response = new Monitor(MonitorKey);

            // Setup
            _httpClient.Setup(x => x.SendAsync<Monitor>(It.IsAny<GetRequest>())).Returns(Task.FromResult(response));

            // Run
            var result = await _client.GetAsync(MonitorKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Key, result.Key);

            // Verify
            _httpClient.Verify(x => x.SendAsync<Monitor>(It.Is<GetRequest>(c =>
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
            _httpClient.Setup(x => x.SendAsync<CreateResponse>(It.IsAny<CreateRequest>())).Returns(Task.FromResult(new CreateResponse { Monitors = new List<Monitor> { (Monitor)monitor } }));

            // Run
            var result = _client.Create(new CreateRequest(monitor));

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ShouldExecuteCreateAsyncMethod()
        {
            var monitor = Job;

            // Setup
            _httpClient.Setup(x => x.SendAsync<CreateResponse>(It.IsAny<CreateRequest>())).Returns(Task.FromResult(new CreateResponse { Monitors = new List<Monitor> { (Monitor)monitor } }));

            // Run
            var result = await _client.CreateAsync(new CreateRequest(monitor));

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
