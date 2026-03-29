namespace Cronitor.Constants
{
    public static class Assertion
    {
        public static MetricAssertion Metric => new MetricAssertion();
        public static ResponseAssertion Response => new ResponseAssertion();
    }

    public class MetricAssertion
    {
        public AssertionBuilder Duration => new AssertionBuilder("metric.duration");
        public AssertionBuilder Count => new AssertionBuilder("metric.count");
        public AssertionBuilder ErrorCount => new AssertionBuilder("metric.error_count");
    }

    public class ResponseAssertion
    {
        public AssertionBuilder Code => new AssertionBuilder("response.code");
        public AssertionBuilder Time => new AssertionBuilder("response.time");
        public AssertionBuilder Body => new AssertionBuilder("response.body");

        public AssertionBuilder Json(string key) => new AssertionBuilder("response.json", key);
        public AssertionBuilder Header(string key) => new AssertionBuilder("response.header", key);
    }

    public class AssertionBuilder
    {
        private readonly string _assertion;
        private readonly string _key;

        public AssertionBuilder(string assertion, string key = null)
        {
            _assertion = assertion;
            _key = key;
        }

        public new string Equals(object value) => Build("=", value);
        public string LessThan(object value) => Build("<", value);
        public string GreaterThan(object value) => Build(">", value);
        public string Contains(object value) => Build("contains", value);

        private string Build(string op, object value)
        {
            return _key != null
                ? $"{_assertion} {_key} {op} {value}"
                : $"{_assertion} {op} {value}";
        }
    }
}
