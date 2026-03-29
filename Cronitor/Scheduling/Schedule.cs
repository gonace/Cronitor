namespace Cronitor.Scheduling
{
    public static class Schedule
    {
        public static CronBuilder Cron => new CronBuilder();
        public static IntervalBuilder Every(int value) => new IntervalBuilder(value);
    }
}
