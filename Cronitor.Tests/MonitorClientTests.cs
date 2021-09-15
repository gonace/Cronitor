using Cronitor.Models;
using Cronitor.Requests.Monitor;
using Xunit;

namespace Cronitor.Tests
{
    public class MonitorClientTests
    {
        private readonly MonitorClient _monitorClient;

        public MonitorClientTests()
        {
            _monitorClient = new MonitorClient("");
        }

        [Fact]
        public void Test()
        {
            //var monitor = new Monitor("v26rUxXl")
            //{
            //    Schedule = "5 * * * *",
            //    Name = "Testing"
            //};
            var request = new DeleteRequest("v26rUxXl");
            _monitorClient.Delete(request);
        }
    }
}
