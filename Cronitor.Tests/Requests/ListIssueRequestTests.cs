using System.Net.Http;
using Cronitor.Constants;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class ListIssueRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new ListIssueRequest();

            Assert.Equal("issues", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new ListIssueRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldDefaultPageToOne()
        {
            var request = new ListIssueRequest();

            Assert.Equal(1, request.Page);
        }

        [Fact]
        public void ShouldBuildUriWithDefaultPage()
        {
            var request = new ListIssueRequest();

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=1", uri.ToString());
        }

        [Fact]
        public void ShouldBuildUriWithCustomPage()
        {
            var request = new ListIssueRequest { Page = 2 };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=2", uri.ToString());
        }

        [Fact]
        public void ShouldRepeatStateForEveryState()
        {
            var request = new ListIssueRequest { States = { IssueState.Unresolved, IssueState.Investigating } };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=1&state=unresolved&state=investigating", uri.ToString());
        }

        [Fact]
        public void ShouldRepeatSeverityForEverySeverity()
        {
            var request = new ListIssueRequest { Severities = { IssueSeverity.Outage, IssueSeverity.MinorOutage } };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=1&severity=outage&severity=minor_outage", uri.ToString());
        }

        [Fact]
        public void ShouldSendExpansionsInLowerCase()
        {
            var request = new ListIssueRequest { WithMonitorDetails = true, WithAlertDetails = false };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=1&withMonitorDetails=true&withAlertDetails=false", uri.ToString());
        }

        [Fact]
        public void ShouldBuildUriWithEveryFilter()
        {
            var request = new ListIssueRequest
            {
                Page = 2,
                PageSize = 500,
                States = { IssueState.Unresolved },
                Severities = { IssueSeverity.Outage },
                Jobs = { "nightly-backup" },
                Checks = { "api-health" },
                Heartbeats = { "queue-worker" },
                Sites = { "storefront" },
                Groups = { "production-jobs" },
                Tags = { "backups" },
                Types = { MonitorType.Job },
                Components = { "database" },
                StatusPage = "public",
                Environment = "production",
                Search = "backup",
                Time = "24h",
                OrderBy = IssueOrder.StartedDescending,
                WithStatusPageDetails = true,
                WithMonitorDetails = true,
                WithAlertDetails = true,
                WithComponentDetails = true,
            };

            var uri = request.ToUri();

            Assert.Equal(
                "https://cronitor.io/api/issues?page=2&pageSize=500&state=unresolved&severity=outage" +
                "&job=nightly-backup&check=api-health&heartbeat=queue-worker&site=storefront&group=production-jobs" +
                "&tag=backups&type=job&component=database&statuspage=public&env=production&search=backup&time=24h" +
                "&orderBy=-started&withStatusPageDetails=true&withMonitorDetails=true&withAlertDetails=true&withComponentDetails=true",
                uri.ToString());
        }

        [Theory]
        [InlineData("started")]
        [InlineData("-started")]
        [InlineData("relevance")]
        [InlineData("-relevance")]
        public void ShouldOrderByEveryDocumentedOrder(string order)
        {
            var orders = new[] { IssueOrder.StartedAscending, IssueOrder.StartedDescending, IssueOrder.RelevanceAscending, IssueOrder.RelevanceDescending };

            Assert.Contains(orders, o => o.ToString() == order);
        }
    }
}
