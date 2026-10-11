using Cronitor.Models;
using System;
using Xunit;
using IssueUpdate = Cronitor.Models.Issues.Event;

namespace Cronitor.Tests;

/// <summary>
/// Fields that only Cronitor sets are never sent, so callers must not be able to set them.
/// </summary>
public class CronitorSetFieldsTests
{
    [Theory]
    [InlineData(typeof(Issue), nameof(Issue.CreatedBy))]
    [InlineData(typeof(Issue), nameof(Issue.CreatedAt))]
    [InlineData(typeof(Issue), nameof(Issue.UpdatedAt))]
    [InlineData(typeof(Notification), nameof(Notification.Monitors))]
    [InlineData(typeof(Notification), nameof(Notification.Status))]
    [InlineData(typeof(Notification), nameof(Notification.CreatedAt))]
    [InlineData(typeof(IssueUpdate), nameof(IssueUpdate.Key))]
    [InlineData(typeof(IssueUpdate), nameof(IssueUpdate.Timestamp))]
    [InlineData(typeof(IssueUpdate), nameof(IssueUpdate.CreatedBy))]
    [InlineData(typeof(IssueUpdate), nameof(IssueUpdate.CreatedAt))]
    [InlineData(typeof(IssueUpdate), nameof(IssueUpdate.UpdatedAt))]
    public void Should_not_let_callers_set_a_field_only_Cronitor_sets(Type model, string field)
    {
        var setter = model.GetProperty(field).SetMethod;

        Assert.False(setter.IsPublic);
    }
}
