namespace Cronitor.Tests.Builders
{
    public static class Make
    {
        public static CheckBuilder Check => new CheckBuilder();
        public static HeartbeatBuilder Heartbeat => new HeartbeatBuilder();
        public static IssueBuilder Issue => new IssueBuilder();
        public static JobBuilder Job => new JobBuilder();
        public static MonitorBuilder Monitor => new MonitorBuilder();
        public static TemplateBuilder Template => new TemplateBuilder();
    }
}