using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Responses;
using Cronitor.Tests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace Cronitor.Tests;

/// <summary>
/// Reads issues from a real Cronitor response, so the models are checked against the API's actual format.
/// </summary>
public class IssueDeserializationTests
{
    [Theory]
    [JsonData("ListIssues.json")]
    public void Should_read_the_state(string json)
    {
        var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

        Assert.Equal("resolved", issue.State.ToString());
        Assert.Null(issue.Severity);
    }

    [Theory]
    [JsonData("ListIssues.json")]
    public void Should_read_the_update_history(string json)
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
    public void Should_read_when_the_issue_ended_and_how_long_it_lasted(string json)
    {
        var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

        Assert.Equal(new DateTime(2026, 10, 11, 0, 30, 34, DateTimeKind.Utc), issue.Ended);
        Assert.Equal(85815, issue.Duration);
    }

    [Fact]
    public void Should_send_an_update_with_only_its_message_and_state()
    {
        var issue = new Issue
        {
            Name = "Database Outage",
            Updates = new[] { new Models.Issues.Event { Message = "Looking into it.", State = IssueState.Investigating } }
        };

        var json = Serializer.Serialize(issue);

        Assert.Contains("\"updates\":[{\"message\":\"Looking into it.\",\"state\":\"investigating\"}]", json);
    }

    [Theory]
    [JsonData("ListIssues.json")]
    public void Should_read_who_created_the_issue_and_when(string json)
    {
        var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

        Assert.Equal("Cronitor Alert", issue.CreatedBy.Name);
        Assert.Equal(new DateTime(2026, 10, 10, 0, 40, 29, DateTimeKind.Utc), issue.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(new DateTime(2026, 10, 11, 0, 30, 44, DateTimeKind.Utc), issue.UpdatedAt.Value, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [JsonData("ListIssues.json")]
    public void Should_read_the_key_and_creation_time_of_an_update(string json)
    {
        var opened = Serializer.Deserialize<ListIssueResponse>(json).Items.Single().Updates.First();

        Assert.Equal("62eff086b0ee2448", opened.Key);
        Assert.Null(opened.CreatedBy);
        Assert.Equal(new DateTime(2026, 10, 10, 0, 40, 29, DateTimeKind.Utc), opened.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(new DateTime(2026, 10, 10, 0, 40, 29, DateTimeKind.Utc), opened.UpdatedAt.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Should_read_the_severity()
    {
        const string json = "{\"key\":\"4b31fca52902b210\",\"severity\":\"outage\",\"state\":\"unresolved\"}";

        var issue = Serializer.Deserialize<Issue>(json);

        Assert.Equal("outage", issue.Severity.ToString());
    }

    [Theory]
    [JsonData("ListIssues.json")]
    public void Should_not_send_what_Cronitor_sets(string json)
    {
        var issue = Serializer.Deserialize<ListIssueResponse>(json).Items.Single();

        var sent = Serializer.Serialize(issue);

        Assert.DoesNotContain("\"created\"", sent);
        Assert.DoesNotContain("\"updated\"", sent);
        Assert.DoesNotContain("\"created_by\"", sent);
    }

    [Fact]
    public void Should_send_state_and_severity_as_strings()
    {
        var issue = new Issue { Name = "Missed Execution", State = IssueState.Unresolved, Severity = IssueSeverity.Outage };

        var json = Serializer.Serialize(issue);

        Assert.Contains("\"state\":\"unresolved\"", json);
        Assert.Contains("\"severity\":\"outage\"", json);
    }
}
