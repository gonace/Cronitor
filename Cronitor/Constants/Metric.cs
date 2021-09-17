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

        //public Metric(string key, string value)
        //{
        //    Key = key;
        //    Value = value;
        //}

        //public Metric WithValue(string value)
        //{
        //    Value = value;

        //    return this;
        //}

        //public Metric WithValue(int value) => WithValue(value.ToString());
        //public Metric WithValue(decimal value) => WithValue(value.ToString(CultureInfo.InvariantCulture));
        //public Metric WithValue(double value) => WithValue(value.ToString(CultureInfo.InvariantCulture));

        //public override string ToString()
        //{
        //    return $"{Key}:{Value}";
        //}

        public override string ToString()
        {
            return Key;
        }
    }
}
