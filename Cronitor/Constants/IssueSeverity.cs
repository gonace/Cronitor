namespace Cronitor.Constants
{
    public class IssueSeverity
    {
        public static readonly IssueSeverity MissingData = new IssueSeverity("missing_data");
        public static readonly IssueSeverity Operational = new IssueSeverity("operational");
        public static readonly IssueSeverity Maintenance = new IssueSeverity("maintenance");
        public static readonly IssueSeverity DegradedPerformance = new IssueSeverity("degraded_performance");
        public static readonly IssueSeverity MinorOutage = new IssueSeverity("minor_outage");
        public static readonly IssueSeverity Outage = new IssueSeverity("outage");

        public string Severity { get; set; }


        public IssueSeverity(string severity)
        {
            Severity = severity;
        }

        public override string ToString()
        {
            return Severity;
        }
    }
}
