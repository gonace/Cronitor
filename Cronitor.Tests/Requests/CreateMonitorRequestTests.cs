using System.Collections.Generic;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class CreateMonitorRequestTests
    {
        [Theory]
        [JsonData("CreateMonitorRequest.json")]
        public async Task ShouldCreateMonitorRequestAsync(string expected)
        {
            var monitor = new Monitor("nightly-backup-job")
                .With(x => x.Type, MonitorType.Job.ToString())
                .With(x => x.Schedule, "0 0 * * *")
                .With(x => x.Notify, new List<string> { "default" })
                .With(x => x.Tags, new List<string> { "nightly" })
                .With(x => x.Assertions, new List<AssertionRule> { Assertion.Metric.Duration.LessThan("15min") })
                .With(x => x.Timezone, "Europe/Stockholm")
                .With(x => x.Note, "note")
                .With(x => x.Platform, "linux")
                .With(x => x.GraceSeconds, 300)
                .With(x => x.FailureTolerance, 3)
                .With(x => x.ScheduleTolerance, 1);
            var request = new CreateMonitorRequest(monitor);

#if NET8_0_OR_GREATER
            var actual = await request.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
#else
            var actual = await request.Content.ReadAsStringAsync();
#endif
            Assert.Equal(expected, actual);
        }
    }
}