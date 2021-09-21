using System.Net.Http;
using Cronitor.Commands;
using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests.Commands
{
    public  class CompleteCommandTests
    {
        [Fact]
        public void ShouldCreateCompleteCommand()
        {
            var command = new CompleteCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("complete", command.Endpoint);
            Assert.Equal("complete", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            var expected = "https://cronitor.link/p/apiKey/monitorKey/complete";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCreateCompleteCommandWithExtendedProperties()
        {
            var command = new CompleteCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey")
                .WithEnvironment("Production")
                .WithHost("127.0.0.1")
                .WithMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
                .WithMetric(Metric.Count, new decimal(99.99))
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("complete", command.Endpoint);
            Assert.Equal("complete", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            var expected = "https://cronitor.link/p/apiKey/monitorKey/complete?env=Production&host=127.0.0.1&message=Lorem ipsum dolor sit amet%2C consectetur adipiscing elit.&metric=count%3A99.99&series=3de5db91-9c02-4e95-b8a9-9a2442702336";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }
    }
}
