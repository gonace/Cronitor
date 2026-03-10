namespace Cronitor.Constants
{
    public class IssueState
    {
        public static readonly IssueState Unresolved = new IssueState("unresolved");
        public static readonly IssueState Investigating = new IssueState("investigating");
        public static readonly IssueState Identified = new IssueState("identified");
        public static readonly IssueState Monitoring = new IssueState("monitoring");
        public static readonly IssueState Resolved = new IssueState("resolved");
        public static readonly IssueState Update = new IssueState("update");

        public string State { get; set; }


        public IssueState(string state)
        {
            State = state;
        }

        public override string ToString()
        {
            return State;
        }
    }
}
