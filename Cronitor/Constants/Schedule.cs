namespace Cronitor.Constants
{
    public static class Schedule
    {
        public static CronBuilder Cron => new CronBuilder();
        public static IntervalBuilder Every(int value) => new IntervalBuilder(value);
    }

    public class IntervalBuilder
    {
        private readonly int _value;

        public IntervalBuilder(int value)
        {
            _value = value;
        }

        public string Seconds => $"every {_value} seconds";
        public string Minutes => $"every {_value} minutes";
        public string Hours => $"every {_value} hours";
        public string Days => $"every {_value} days";
    }

    public class CronBuilder
    {
        /// <summary>
        /// Every minute: * * * * *
        /// </summary>
        public string EveryMinute() => "* * * * *";

        /// <summary>
        /// Every hour at the given minute: {minute} * * * *
        /// </summary>
        public string EveryHour(int minute = 0) => $"{minute} * * * *";

        /// <summary>
        /// Daily at the given time: {minute} {hour} * * *
        /// </summary>
        public string Daily(int hour = 0, int minute = 0) => $"{minute} {hour} * * *";

        /// <summary>
        /// Weekly on the given day at the given time: {minute} {hour} * * {day}
        /// </summary>
        public string Weekly(int day, int hour = 0, int minute = 0) => $"{minute} {hour} * * {day}";

        /// <summary>
        /// Monthly on the given day at the given time: {minute} {hour} {day} * *
        /// </summary>
        public string Monthly(int day, int hour = 0, int minute = 0) => $"{minute} {hour} {day} * *";

        /// <summary>
        /// Yearly on the given month and day at the given time: {minute} {hour} {day} {month} *
        /// </summary>
        public string Yearly(int month, int day, int hour = 0, int minute = 0) => $"{minute} {hour} {day} {month} *";

        /// <summary>
        /// Pass a raw cron expression through as-is.
        /// </summary>
        public string Expression(string expression) => expression;

        /// <summary>
        /// Start building a custom cron expression field by field.
        /// </summary>
        public CronFieldBuilder Create() => new CronFieldBuilder();
    }

    public class CronFieldBuilder
    {
        private string _minute = "*";
        private string _hour = "*";
        private string _dayOfMonth = "*";
        private string _month = "*";
        private string _dayOfWeek = "*";

        public CronFieldBuilder Minute(int value) { _minute = value.ToString(); return this; }
        public CronFieldBuilder Minute(string value) { _minute = value; return this; }

        public CronFieldBuilder Hour(int value) { _hour = value.ToString(); return this; }
        public CronFieldBuilder Hour(string value) { _hour = value; return this; }

        public CronFieldBuilder DayOfMonth(int value) { _dayOfMonth = value.ToString(); return this; }
        public CronFieldBuilder DayOfMonth(string value) { _dayOfMonth = value; return this; }

        public CronFieldBuilder Month(int value) { _month = value.ToString(); return this; }
        public CronFieldBuilder Month(string value) { _month = value; return this; }

        public CronFieldBuilder DayOfWeek(int value) { _dayOfWeek = value.ToString(); return this; }
        public CronFieldBuilder DayOfWeek(string value) { _dayOfWeek = value; return this; }

        /// <summary>
        /// Set a step value for the minute field: */{step} or {value}/{step}
        /// </summary>
        public CronFieldBuilder MinuteEvery(int step) { _minute = $"*/{step}"; return this; }

        /// <summary>
        /// Set a step value for the hour field: */{step} or {value}/{step}
        /// </summary>
        public CronFieldBuilder HourEvery(int step) { _hour = $"*/{step}"; return this; }

        /// <summary>
        /// Set a range for the minute field: {from}-{to}
        /// </summary>
        public CronFieldBuilder MinuteRange(int from, int to) { _minute = $"{from}-{to}"; return this; }

        /// <summary>
        /// Set a range for the hour field: {from}-{to}
        /// </summary>
        public CronFieldBuilder HourRange(int from, int to) { _hour = $"{from}-{to}"; return this; }

        /// <summary>
        /// Set a range for the day of month field: {from}-{to}
        /// </summary>
        public CronFieldBuilder DayOfMonthRange(int from, int to) { _dayOfMonth = $"{from}-{to}"; return this; }

        /// <summary>
        /// Set a range for the month field: {from}-{to}
        /// </summary>
        public CronFieldBuilder MonthRange(int from, int to) { _month = $"{from}-{to}"; return this; }

        /// <summary>
        /// Set a range for the day of week field: {from}-{to}
        /// </summary>
        public CronFieldBuilder DayOfWeekRange(int from, int to) { _dayOfWeek = $"{from}-{to}"; return this; }

        public string Build() => $"{_minute} {_hour} {_dayOfMonth} {_month} {_dayOfWeek}";

        public override string ToString() => Build();
    }
}
