using Cronitor.Commands;
using Cronitor.Constants;
using System.Net.Http;
using Xunit;

namespace Cronitor.Tests.Commands
{
    public class FailCommandTests
    {
        [Fact]
        public void ShouldCreateFailCommand()
        {
            var command = new FailCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("fail", command.Endpoint);
            Assert.Equal("fail", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/fail";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCreateFailCommandWithExtendedProperties()
        {
            var command = new FailCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey")
                .WithEnvironment("Production")
                .WithHost("127.0.0.1")
                .WithMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
                .WithMetric(Metric.Count, new decimal(99.99))
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("fail", command.Endpoint);
            Assert.Equal("fail", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/fail?env=Production&host=127.0.0.1&message='Lorem ipsum dolor sit amet, consectetur adipiscing elit.'&metric=count:99.99&series=3de5db91-9c02-4e95-b8a9-9a2442702336";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }
    }
}
