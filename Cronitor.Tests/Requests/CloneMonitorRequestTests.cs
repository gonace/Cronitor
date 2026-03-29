using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class CloneMonitorRequestTests
    {
        [Theory]
        [JsonData("CloneMonitorRequest.json")]
        public async Task ShouldCreateCloneMonitorRequestAsync(string expected)
        {
            var request = new CloneMonitorRequest("nightly-backup-job", "nightly-backup-job-clone");

#if NET8_0_OR_GREATER
            var actual = await request.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
#else
            var actual = await request.Content.ReadAsStringAsync();
#endif
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new CloneMonitorRequest("test");

            Assert.Equal("monitors/clone", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToPost()
        {
            var request = new CloneMonitorRequest("test");

            Assert.Equal(HttpMethod.Post, request.Method);
        }

        [Fact]
        public void ShouldBuildUri()
        {
            var request = new CloneMonitorRequest("test");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors/clone", uri.ToString());
        }
    }
}