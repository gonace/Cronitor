using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests
{
    public class RequestToUriTests : BaseTest
    {
        [Fact]
        public void PauseMonitorRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new PauseMonitorRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}/pause", uri.ToString());
        }

        [Fact]
        public void PauseMonitorRequest_ToUri_WithDuration_ShouldIncludeDuration()
        {
            var request = new PauseMonitorRequest(MonitorKey, 24);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}/pause/24", uri.ToString());
        }

        [Fact]
        public void UnpauseMonitorRequest_ToUri_ShouldIncludeDurationZero()
        {
            var request = new UnpauseMonitorRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}/pause/0", uri.ToString());
        }

        [Fact]
        public void GetMonitorRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new GetMonitorRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}", uri.ToString());
        }

        [Fact]
        public void DeleteMonitorRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new DeleteMonitorRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}", uri.ToString());
        }

        [Fact]
        public void ListActivitiesRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new ListActivitiesRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}/activity", uri.ToString());
        }

        [Fact]
        public void ListAlertsRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new ListAlertsRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}/alerts", uri.ToString());
        }

        [Fact]
        public void ListPingsRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new ListPingsRequest(MonitorKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"monitors/{MonitorKey}/pings", uri.ToString());
        }

        [Fact]
        public void GetIssueRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            const string issueKey = "issue-123";
            var request = new GetIssueRequest(issueKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"issues/{issueKey}", uri.ToString());
        }

        [Fact]
        public void UpdateIssueRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            const string issueKey = "issue-123";
            var request = new UpdateIssueRequest(issueKey, new Models.Issue());
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"issues/{issueKey}", uri.ToString());
        }

        [Fact]
        public void GetNotificationRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new GetNotificationRequest(TemplateKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"templates/{TemplateKey}", uri.ToString());
        }

        [Fact]
        public void DeleteNotificationRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new DeleteNotificationRequest(TemplateKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"templates/{TemplateKey}", uri.ToString());
        }

        [Fact]
        public void UpdateNotificationRequest_ToUri_ShouldReplaceKeyPlaceholder()
        {
            var request = new UpdateNotificationRequest(TemplateKey, new Models.Template());
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains($"templates/{TemplateKey}", uri.ToString());
        }

        [Fact]
        public void ListMonitorRequest_ToUri_ShouldIncludePageInQueryString()
        {
            var request = new ListMonitorRequest { Page = 2 };
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains("monitors", uri.ToString());
            Assert.Contains("page=2", uri.ToString());
        }

        [Fact]
        public void ListIssueRequest_ToUri_ShouldIncludePageInQueryString()
        {
            var request = new ListIssueRequest { Page = 3 };
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains("issues", uri.ToString());
            Assert.Contains("page=3", uri.ToString());
        }

        [Fact]
        public void PauseMonitorRequest_ToUri_WithSpecialCharacters_ShouldEncodeCorrectly()
        {
            const string specialKey = "monitor-with-special-chars";
            var request = new PauseMonitorRequest(specialKey);
            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains(specialKey, uri.ToString());
        }
    }
}
