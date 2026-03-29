namespace Cronitor.Assertions
{
    public class MetricAssertion
    {
        public AssertionBuilder Duration => new AssertionBuilder("metric.duration");
        public AssertionBuilder Count => new AssertionBuilder("metric.count");
        public AssertionBuilder ErrorCount => new AssertionBuilder("metric.error_count");
    }
}