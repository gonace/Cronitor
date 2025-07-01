using Cronitor.Clients;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Tests.Builders;
using Xunit;

namespace Cronitor.Tests.Clients
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
                .Schedule("every 60 seconds")
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
    }
}
