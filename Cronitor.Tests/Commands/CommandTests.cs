using Cronitor.Commands;
using Cronitor.Constants;
using System;
using System.Net.Http;
using Xunit;

namespace Cronitor.Tests.Commands
{
    public class CommandTests
    {
        [Fact]
        public void ShouldCreateCustomCommand()
        {
            var command = new Command(HttpMethod.Options, "custom")
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("custom", command.Endpoint);
            Assert.Equal("custom", command.ToString());
            Assert.Equal(HttpMethod.Options, command.Method);

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/custom";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCreateCustomCommandWithExtendedProperties()
        {
            var command = new Command(HttpMethod.Options, "custom")
            {
                Environment = "Production",
                Host = "127.0.0.1",
                Series = "3de5db91-9c02-4e95-b8a9-9a2442702336"
            }
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey")
                .WithMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
                .WithMetric(Metric.Count, 100);

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("custom", command.Endpoint);
            Assert.Equal("custom", command.ToString());

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/custom?env=Production&host=127.0.0.1&message='Lorem ipsum dolor sit amet, consectetur adipiscing elit.'&metric=count:100&series=3de5db91-9c02-4e95-b8a9-9a2442702336";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldThrowExceptionWhenMissingRequiredFields()
        {
            var command = new Command(HttpMethod.Get, "foo/bar");

            Assert.Throws<ArgumentException>(() => command.ToUrl());
        }
    }
}
