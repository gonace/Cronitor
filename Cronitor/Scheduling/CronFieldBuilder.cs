namespace Cronitor.Scheduling
{
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
        /// Set a step value for the minute field: */{step}
        /// </summary>
        public CronFieldBuilder MinuteEvery(int step) { _minute = $"*/{step}"; return this; }

        /// <summary>
        /// Set a step value for the hour field: */{step}
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

        public ScheduleExpression Build() => new ScheduleExpression($"{_minute} {_hour} {_dayOfMonth} {_month} {_dayOfWeek}");

        public override string ToString() => Build().Value;
    }
}
