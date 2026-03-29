using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Constants.Scheduling;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class UpdateMonitorRequestTests
    {
        [Theory]
        [JsonData("UpdateMonitorRequest.json")]
        public async Task ShouldCreateUpdateMonitorRequestAsync(string expected)
        {
            var monitor = new Monitor("nightly-backup-job")
                .With(x => x.Type, MonitorType.Job.ToString())
                .With(x => x.Schedule, new ScheduleExpression("0 0 * * *"))
                .With(x => x.Notify, new List<string> { "default" })
                .With(x => x.Tags, new List<string> { "nightly" })
                .With(x => x.Assertions, new List<string> { "metric.duration < 15min" })
                .With(x => x.Timezone, "Europe/Stockholm")
                .With(x => x.Note, "note")
                .With(x => x.Platform, "linux");
            var request = new UpdateMonitorRequest(monitor);

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
            var monitor = new Monitor("test");
            var request = new UpdateMonitorRequest(monitor);

            Assert.Equal("monitors", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToPut()
        {
            var monitor = new Monitor("test");
            var request = new UpdateMonitorRequest(monitor);

            Assert.Equal(HttpMethod.Put, request.Method);
        }

        [Fact]
        public void ShouldBuildUri()
        {
            var monitor = new Monitor("test");
            var request = new UpdateMonitorRequest(monitor);

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors", uri.ToString());
        }
    }
}
