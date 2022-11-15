using Cronitor.Commands;
using Cronitor.Constants;
using System.Net.Http;
using Xunit;

namespace Cronitor.Tests.Commands
{
    public class TickCommandTests
    {
        [Fact]
        public void ShouldCreateTickCommand()
        {
            var command = new TickCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("tick", command.Endpoint);
            Assert.Equal("tick", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/tick";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCreateTickCommandWithExtendedProperties()
        {
            var command = new TickCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey")
                .WithEnvironment("Production")
                .WithHost("127.0.0.1")
                .WithMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
                .WithMetric(Metric.Count, 9.99)
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("tick", command.Endpoint);
            Assert.Equal("tick", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/tick?env=Production&host=127.0.0.1&message='Lorem ipsum dolor sit amet, consectetur adipiscing elit.'&metric=count:9.99&series=3de5db91-9c02-4e95-b8a9-9a2442702336";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }
    }
}
