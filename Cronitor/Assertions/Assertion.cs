namespace Cronitor.Assertions
{
    public static class Assertion
    {
        public static MetricAssertion Metric => new MetricAssertion();
        public static ResponseAssertion Response => new ResponseAssertion();
    }
}