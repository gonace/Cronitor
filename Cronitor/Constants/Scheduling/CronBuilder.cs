namespace Cronitor.Constants.Scheduling
{
    public class CronBuilder
    {
        /// <summary>
        /// Every minute: * * * * *
        /// </summary>
        public ScheduleExpression EveryMinute() => new ScheduleExpression("* * * * *");

        /// <summary>
        /// Every hour at the given minute: {minute} * * * *
        /// </summary>
        public ScheduleExpression EveryHour(int minute = 0) => new ScheduleExpression($"{minute} * * * *");

        /// <summary>
        /// Daily at the given time: {minute} {hour} * * *
        /// </summary>
        public ScheduleExpression Daily(int hour = 0, int minute = 0) => new ScheduleExpression($"{minute} {hour} * * *");

        /// <summary>
        /// Weekly on the given day at the given time: {minute} {hour} * * {day}
        /// </summary>
        public ScheduleExpression Weekly(int day, int hour = 0, int minute = 0) => new ScheduleExpression($"{minute} {hour} * * {day}");

        /// <summary>
        /// Monthly on the given day at the given time: {minute} {hour} {day} * *
        /// </summary>
        public ScheduleExpression Monthly(int day, int hour = 0, int minute = 0) => new ScheduleExpression($"{minute} {hour} {day} * *");

        /// <summary>
        /// Yearly on the given month and day at the given time: {minute} {hour} {day} {month} *
        /// </summary>
        public ScheduleExpression Yearly(int month, int day, int hour = 0, int minute = 0) => new ScheduleExpression($"{minute} {hour} {day} {month} *");

        /// <summary>
        /// Pass a raw cron expression through as-is.
        /// </summary>
        public ScheduleExpression Expression(string expression) => new ScheduleExpression(expression);

        /// <summary>
        /// Start building a custom cron expression field by field.
        /// </summary>
        public CronFieldBuilder Create() => new CronFieldBuilder();
    }
}
