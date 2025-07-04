﻿using Cronitor.Commands;
using Cronitor.Constants;
using System.Net.Http;
using Xunit;

namespace Cronitor.Tests
{
    public class RunCommandTests
    {
        [Fact]
        public void ShouldCreateRunCommand()
        {
            var command = new RunCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("run", command.Endpoint);
            Assert.Equal("run", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/run", command.ToUrl());
        }

        [Fact]
        public void ShouldCreateRunCommandWithExtendedProperties()
        {
            var command = new RunCommand()
                .WithApiKey("apiKey")
                .WithMonitorKey("monitorKey")
                .WithEnvironment("Production")
                .WithHost("127.0.0.1")
                .WithMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
                .WithMetric(Metric.Count, new decimal(99.99))
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("run", command.Endpoint);
            Assert.Equal("run", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);

            const string expected = "https://cronitor.link/p/apiKey/monitorKey/run?env=Production&host=127.0.0.1&message='Lorem ipsum dolor sit amet, consectetur adipiscing elit.'&metric=count:99.99&series=3de5db91-9c02-4e95-b8a9-9a2442702336";
            var actual = command.ToUrl();

            Assert.Equal(expected, actual);
        }
    }
}
