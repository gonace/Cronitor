namespace Cronitor.Constants
{
    public class IssueState
    {
        public static readonly IssueSeverity unresolved = new IssueSeverity("unresolved");
        public static readonly IssueSeverity investigating = new IssueSeverity("investigating");
        public static readonly IssueSeverity identified = new IssueSeverity("identified");
        public static readonly IssueSeverity monitoring = new IssueSeverity("monitoring");
        public static readonly IssueSeverity resolved = new IssueSeverity("resolved");
        public static readonly IssueSeverity update = new IssueSeverity("update");

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
