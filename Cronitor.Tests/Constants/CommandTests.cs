using System;
using System.Net.Http;
using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests.Constants
{
    public  class CommandTests
    {
        [Fact]
        public void ShouldCreateCompleteCommand()
        {
            var command = Command.Complete
                .SetApiKey("apiKey")
                .SetMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("complete", command.Endpoint);
            Assert.Equal("complete", command.ToString());
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/complete", command.ToUrl());
        }

        [Fact]
        public void ShouldCreateFailCommand()
        {
            var command = Command.Fail
                .SetApiKey("apiKey")
                .SetMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("fail", command.Endpoint);
            Assert.Equal("fail", command.ToString());
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/fail", command.ToUrl());
        }

        [Fact]
        public void ShouldCreateRunCommand()
        {
            var command = Command.Run
                .SetApiKey("apiKey")
                .SetMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("run", command.Endpoint);
            Assert.Equal("run", command.ToString());
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/run", command.ToUrl());
        }

        [Fact]
        public void ShouldCreateTickCommand()
        {
            var command = Command.Tick
                .SetApiKey("apiKey")
                .SetMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("tick", command.Endpoint);
            Assert.Equal("tick", command.ToString());
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/tick", command.ToUrl());
        }

        [Fact]
        public void ShouldCreateCustomCommand()
        {
            var command = new Command(HttpMethod.Options, "custom")
                .SetApiKey("apiKey")
                .SetMonitorKey("monitorKey");

            Assert.Equal("apiKey", command.ApiKey);
            Assert.Equal("monitorKey", command.MonitorKey);
            Assert.Equal("custom", command.Endpoint);
            Assert.Equal("custom", command.ToString());
            Assert.Equal("https://cronitor.link/p/apiKey/monitorKey/custom", command.ToUrl());
        }

        [Fact]
        public void ShouldThrowException()
        {
            var command = Command.Complete;

            Assert.Throws<ArgumentException>(() => command.ToUrl());
        }
    }
}
