using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests
{
    public class ScheduleTests
    {
        [Fact]
        public void EveryMinute()
        {
            var result = Schedule.Cron.EveryMinute();

            Assert.Equal("* * * * *", result);
        }

        [Fact]
        public void EveryHourAtZero()
        {
            var result = Schedule.Cron.EveryHour();

            Assert.Equal("0 * * * *", result);
        }

        [Fact]
        public void EveryHourAtMinute()
        {
            var result = Schedule.Cron.EveryHour(minute: 30);

            Assert.Equal("30 * * * *", result);
        }

        [Fact]
        public void DailyAtMidnight()
        {
            var result = Schedule.Cron.Daily();

            Assert.Equal("0 0 * * *", result);
        }

        [Fact]
        public void DailyAtHour()
        {
            var result = Schedule.Cron.Daily(hour: 3);

            Assert.Equal("0 3 * * *", result);
        }

        [Fact]
        public void DailyAtHourAndMinute()
        {
            var result = Schedule.Cron.Daily(hour: 0, minute: 35);

            Assert.Equal("35 0 * * *", result);
        }

        [Fact]
        public void WeeklyOnDay()
        {
            var result = Schedule.Cron.Weekly(day: 1);

            Assert.Equal("0 0 * * 1", result);
        }

        [Fact]
        public void WeeklyOnDayAtTime()
        {
            var result = Schedule.Cron.Weekly(day: 1, hour: 8, minute: 30);

            Assert.Equal("30 8 * * 1", result);
        }

        [Fact]
        public void MonthlyOnDay()
        {
            var result = Schedule.Cron.Monthly(day: 15);

            Assert.Equal("0 0 15 * *", result);
        }

        [Fact]
        public void MonthlyOnDayAtTime()
        {
            var result = Schedule.Cron.Monthly(day: 1, hour: 6, minute: 15);

            Assert.Equal("15 6 1 * *", result);
        }

        [Fact]
        public void YearlyOnDate()
        {
            var result = Schedule.Cron.Yearly(month: 1, day: 1);

            Assert.Equal("0 0 1 1 *", result);
        }

        [Fact]
        public void YearlyOnDateAtTime()
        {
            var result = Schedule.Cron.Yearly(month: 6, day: 15, hour: 12, minute: 30);

            Assert.Equal("30 12 15 6 *", result);
        }

        [Fact]
        public void RawExpression()
        {
            var result = Schedule.Cron.Expression("0 0 * * *");

            Assert.Equal("0 0 * * *", result);
        }

        [Fact]
        public void EverySeconds()
        {
            var result = Schedule.Every(60).Seconds;

            Assert.Equal("every 60 seconds", result);
        }

        [Fact]
        public void EveryMinutes()
        {
            var result = Schedule.Every(5).Minutes;

            Assert.Equal("every 5 minutes", result);
        }

        [Fact]
        public void EveryHours()
        {
            var result = Schedule.Every(2).Hours;

            Assert.Equal("every 2 hours", result);
        }

        [Fact]
        public void EveryDays()
        {
            var result = Schedule.Every(1).Days;

            Assert.Equal("every 1 days", result);
        }

        [Fact]
        public void CustomCronBuilder()
        {
            var result = Schedule.Cron.Create()
                .Minute(30)
                .Hour(3)
                .DayOfMonth(15)
                .Month(6)
                .DayOfWeek(1)
                .Build();

            Assert.Equal("30 3 15 6 1", result);
        }

        [Fact]
        public void CustomCronBuilderDefaults()
        {
            var result = Schedule.Cron.Create()
                .Minute(0)
                .Hour(3)
                .Build();

            Assert.Equal("0 3 * * *", result);
        }

        [Fact]
        public void CustomCronBuilderWithStep()
        {
            var result = Schedule.Cron.Create()
                .MinuteEvery(15)
                .Build();

            Assert.Equal("*/15 * * * *", result);
        }

        [Fact]
        public void CustomCronBuilderWithRange()
        {
            var result = Schedule.Cron.Create()
                .Minute(0)
                .HourRange(9, 17)
                .DayOfWeekRange(1, 5)
                .Build();

            Assert.Equal("0 9-17 * * 1-5", result);
        }

        [Fact]
        public void CustomCronBuilderToString()
        {
            var result = Schedule.Cron.Create()
                .Minute(0)
                .Hour(12)
                .ToString();

            Assert.Equal("0 12 * * *", result);
        }
    }
}
