using System.Net.Http;
using Cronitor.Commands;
using Cronitor.Constants;
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
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/tick", command.ToUrl());
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
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336")
                .WithMetric(Metric.Count, 9.99);

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("tick", command.Endpoint);
            Assert.Equal("tick", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/tick?env=Production&host=127.0.0.1&message=Lorem ipsum dolor sit amet%2C consectetur adipiscing elit.&metric=count%3A9.99&series=3de5db91-9c02-4e95-b8a9-9a2442702336", command.ToUrl());
        }
    }
}
