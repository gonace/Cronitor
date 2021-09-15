namespace Cronitor.Constants
{
    public class MonitorType
    {
        public static MonitorType Check = new MonitorType("check");
        public static MonitorType Event = new MonitorType("event");
        public static MonitorType Job = new MonitorType("job");

        public string Value { get; }

        private MonitorType(string value)
        {
            Value = value; ;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
