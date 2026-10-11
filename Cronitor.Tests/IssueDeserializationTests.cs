using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace Cronitor.Tests
{
    /// <summary>
    /// Reads issues from a real Cronitor response, so the models are checked against the API's actual format.
    /// </summary>
    public class IssueDeserializationTests
    {
        [Theory]
        [JsonData("ListIssues.json")]
        public void ShouldReadTheState(string json)
        {
            var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

            Assert.Equal("resolved", issue.State.ToString());
            Assert.Null(issue.Severity);
        }

        [Theory]
        [JsonData("ListIssues.json")]
        public void ShouldReadTheUpdateHistory(string json)
        {
            var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

            Assert.Equal(2, issue.Updates.Count());
            var opened = issue.Updates.First();
            Assert.Equal("Issue opened on monitor failure.", opened.Message);
            Assert.Equal("unresolved", opened.State.ToString());
            Assert.Equal(new DateTime(2026, 10, 10, 0, 40, 19, DateTimeKind.Utc), opened.Timestamp);
        }

        [Theory]
        [JsonData("ListIssues.json")]
        public void ShouldReadWhenTheIssueEndedAndHowLongItLasted(string json)
        {
            var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

            Assert.Equal(new DateTime(2026, 10, 11, 0, 30, 34, DateTimeKind.Utc), issue.Ended);
            Assert.Equal(85815, issue.Duration);
        }

        [Fact]
        public void ShouldSendAnUpdateWithOnlyItsMessageAndState()
        {
            var issue = new Issue
            {
                Name = "Database Outage",
                Updates = new[] { new Models.Issues.Event { Message = "Looking into it.", State = IssueState.Investigating } }
            };

            var json = Serializer.Serialize(issue);

            Assert.Contains("\"updates\":[{\"message\":\"Looking into it.\",\"state\":\"investigating\"}]", json);
        }

        [Fact]
        public void ShouldReadTheSeverity()
        {
            const string json = "{\"key\":\"4b31fca52902b210\",\"severity\":\"outage\",\"state\":\"unresolved\"}";

            var issue = Serializer.Deserialize<Issue>(json);

            Assert.Equal("outage", issue.Severity.ToString());
        }

        [Fact]
        public void ShouldSendStateAndSeverityAsStrings()
        {
            var issue = new Issue { Name = "Missed Execution", State = IssueState.Unresolved, Severity = IssueSeverity.Outage };

            var json = Serializer.Serialize(issue);

            Assert.Contains("\"state\":\"unresolved\"", json);
            Assert.Contains("\"severity\":\"outage\"", json);
        }
    }
}
