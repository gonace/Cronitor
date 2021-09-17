using System.Net.Http;
using Cronitor.Commands;
using Cronitor.Constants;
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
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/fail", command.ToUrl());
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
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336")
                .WithMetric(Metric.Count, new decimal(99.99));

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("fail", command.Endpoint);
            Assert.Equal("fail", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/fail?host=127.0.0.1&message=Lorem ipsum dolor sit amet%2C consectetur adipiscing elit.&env=Production&series=3de5db91-9c02-4e95-b8a9-9a2442702336&metric=count%3A99.99", command.ToUrl());
        }
    }
}
