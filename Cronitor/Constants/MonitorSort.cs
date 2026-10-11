namespace Cronitor.Constants
{
    /// <summary>
    /// The order of a monitor list.
    /// </summary>
    public class MonitorSort
    {
        public static readonly MonitorSort CreatedAscending = new MonitorSort("created");
        public static readonly MonitorSort CreatedDescending = new MonitorSort("-created");
        public static readonly MonitorSort NameAscending = new MonitorSort("name");
        public static readonly MonitorSort NameDescending = new MonitorSort("-name");

        private MonitorSort(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public override string ToString()
        {
            return Key;
        }
    }
}
