﻿using System.Net.Http;
using Cronitor.Commands;
using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests.Commands
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
                .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336")
                .WithMetric(Metric.Count, new decimal(99.99));

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("run", command.Endpoint);
            Assert.Equal("run", command.ToString());
            Assert.Equal(HttpMethod.Get, command.Method);
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/run?host=127.0.0.1&message=Lorem ipsum dolor sit amet%2C consectetur adipiscing elit.&env=Production&series=3de5db91-9c02-4e95-b8a9-9a2442702336&metric=count%3A99.99", command.ToUrl());
        }
    }
}
