namespace Cronitor.Constants
{
    public class MonitorType
    {
        public static readonly MonitorType Job = new MonitorType("job");
        public static readonly MonitorType Check = new MonitorType("check");
        public static readonly MonitorType Heartbeat = new MonitorType("heartbeat");
        public static readonly MonitorType Site = new MonitorType("site");


        public string Key { get; set; }


        public MonitorType(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
