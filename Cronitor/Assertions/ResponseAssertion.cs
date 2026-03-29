namespace Cronitor.Assertions
{
    public class ResponseAssertion
    {
        public AssertionBuilder Code => new AssertionBuilder("response.code");
        public AssertionBuilder Time => new AssertionBuilder("response.time");
        public AssertionBuilder Body => new AssertionBuilder("response.body");

        public AssertionBuilder Json(string key) => new AssertionBuilder("response.json", key);
        public AssertionBuilder Header(string key) => new AssertionBuilder("response.header", key);
    }
}