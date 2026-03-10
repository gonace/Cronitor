 using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests
{
    public class IssueStateTests
    {
        [Fact]
        public void ShouldHaveUnresolvedState()
        {
            Assert.NotNull(IssueState.Unresolved);
            Assert.Equal("unresolved", IssueState.Unresolved.State);
        }

        [Fact]
        public void ShouldHaveInvestigatingState()
        {
            Assert.NotNull(IssueState.Investigating);
            Assert.Equal("investigating", IssueState.Investigating.State);
        }

        [Fact]
        public void ShouldHaveIdentifiedState()
        {
            Assert.NotNull(IssueState.Identified);
            Assert.Equal("identified", IssueState.Identified.State);
        }

        [Fact]
        public void ShouldHaveMonitoringState()
        {
            Assert.NotNull(IssueState.Monitoring);
            Assert.Equal("monitoring", IssueState.Monitoring.State);
        }

        [Fact]
        public void ShouldHaveResolvedState()
        {
            Assert.NotNull(IssueState.Resolved);
            Assert.Equal("resolved", IssueState.Resolved.State);
        }

        [Fact]
        public void ShouldHaveUpdateState()
        {
            Assert.NotNull(IssueState.Update);
            Assert.Equal("update", IssueState.Update.State);
        }

        [Fact]
        public void ToStringShouldReturnState()
        {
            var issueState = new IssueState("custom-state");

            Assert.Equal("custom-state", issueState.ToString());
        }

        [Fact]
        public void ToStringShouldReturnStateForStaticInstances()
        {
            Assert.Equal("unresolved", IssueState.Unresolved.ToString());
            Assert.Equal("investigating", IssueState.Investigating.ToString());
            Assert.Equal("identified", IssueState.Identified.ToString());
            Assert.Equal("monitoring", IssueState.Monitoring.ToString());
            Assert.Equal("resolved", IssueState.Resolved.ToString());
            Assert.Equal("update", IssueState.Update.ToString());
        }
    }
}
