namespace Cronitor.Constants
{
    public class Metric
    {
        public static readonly Metric Count = new Metric("count");
        public static readonly Metric Duration = new Metric("duration");
        public static readonly Metric Errors = new Metric("error_count");

        public string Key { get; set; }


        public Metric(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
