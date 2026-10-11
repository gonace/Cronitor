using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests;

public class IssueStateTests
{
    [Fact]
    public void Should_have_the_Unresolved_state()
    {
        Assert.NotNull(IssueState.Unresolved);
        Assert.Equal("unresolved", IssueState.Unresolved.State);
    }

    [Fact]
    public void Should_have_the_Investigating_state()
    {
        Assert.NotNull(IssueState.Investigating);
        Assert.Equal("investigating", IssueState.Investigating.State);
    }

    [Fact]
    public void Should_have_the_Identified_state()
    {
        Assert.NotNull(IssueState.Identified);
        Assert.Equal("identified", IssueState.Identified.State);
    }

    [Fact]
    public void Should_have_the_Monitoring_state()
    {
        Assert.NotNull(IssueState.Monitoring);
        Assert.Equal("monitoring", IssueState.Monitoring.State);
    }

    [Fact]
    public void Should_have_the_Resolved_state()
    {
        Assert.NotNull(IssueState.Resolved);
        Assert.Equal("resolved", IssueState.Resolved.State);
    }

    [Fact]
    public void Should_have_the_Update_state()
    {
        Assert.NotNull(IssueState.Update);
        Assert.Equal("update", IssueState.Update.State);
    }

    [Fact]
    public void Should_return_the_state_from_ToString()
    {
        var issueState = new IssueState("custom-state");

        Assert.Equal("custom-state", issueState.ToString());
    }

    [Fact]
    public void Should_return_the_state_from_ToString_for_static_instances()
    {
        Assert.Equal("unresolved", IssueState.Unresolved.ToString());
        Assert.Equal("investigating", IssueState.Investigating.ToString());
        Assert.Equal("identified", IssueState.Identified.ToString());
        Assert.Equal("monitoring", IssueState.Monitoring.ToString());
        Assert.Equal("resolved", IssueState.Resolved.ToString());
        Assert.Equal("update", IssueState.Update.ToString());
    }
}
