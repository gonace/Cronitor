namespace Cronitor.Constants
{
    public class Region
    {
        public static readonly Region Bahrain = new Region("Bahrain", "me-south-1");
        public static readonly Region California = new Region("California, USA", "us-west-1");
        public static readonly Region Dublin = new Region("Dublin, Ireland", "eu-west-1");
        public static readonly Region Frankfurt = new Region("Frankfurt, Germany", "eu-central-1");
        public static readonly Region Mumbai = new Region("Mumbai, India", "ap-south-1");
        public static readonly Region Ohio = new Region("Ohio, USA", "us-east-2");
        public static readonly Region SaoPaulo = new Region("São Paulo, Brazil", "sa-east-1");
        public static readonly Region Singapore = new Region("Singapore", "ap-southeast-1");
        public static readonly Region Stockholm = new Region("Stockholm, Sweden", "eu-north-1");
        public static readonly Region Sydney = new Region("Sydney, Australia", "ap-southeast-2");
        public static readonly Region Tokyo = new Region("Tokyo, Japan", "ap-northeast-1");
        public static readonly Region Virginia = new Region("Virginia, USA", "us-east-1");

        public string Key { get; set; }
        public string Value { get; set; }


        public Region(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
