using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests
{
    public class ServiceTests : BaseTest
    {
        [Fact]
        public void ShouldThrowExceptionIfNotConfigured()
        {
            var service = new Service();

            Assert.Throws<NotConfiguredException>(() => service.Issues);
            Assert.Throws<NotConfiguredException>(() => service.Monitors);
            Assert.Throws<NotConfiguredException>(() => service.Notifications);
            Assert.Throws<NotConfiguredException>(() => service.Telemetries);
        }

        [Fact]
        public void ShouldConfigureService()
        {
            var service = new Service();
            service.Configure(ApiKey);

            Assert.NotNull(service.Issues);
            Assert.NotNull(service.Monitors);
            Assert.NotNull(service.Notifications);
            Assert.NotNull(service.Telemetries);
        }

        [Fact]
        public void ShouldCreateAllClients()
        {
            var service = new Service(ApiKey);

            Assert.NotNull(service.Issues);
            Assert.NotNull(service.Monitors);
            Assert.NotNull(service.Notifications);
            Assert.NotNull(service.Telemetries);
        }

        [Fact]
        public void ShouldSendThroughTheGivenHttpClient()
        {
            var handler = new FakeHttpMessageHandler();
            var service = new Service(ApiKey, new HttpClient(Urls.DefaultApiUrl, ApiKey, handler));

            service.Telemetries.Run(MonitorKey);
            service.Monitors.Delete(MonitorKey);

            Assert.Equal(2, handler.Requests.Count);
        }

        [Fact]
        public void ShouldDisposeAllClients()
        {
            using (var service = new Service())
            {
                service.Configure(ApiKey);

                var issues = service.Issues;
                var monitors = service.Monitors;
                var notifications = service.Notifications;
                var telemetries = service.Telemetries;

                Assert.NotNull(issues);
                Assert.NotNull(monitors);
                Assert.NotNull(notifications);
                Assert.NotNull(telemetries);
            }
        }
    }
}