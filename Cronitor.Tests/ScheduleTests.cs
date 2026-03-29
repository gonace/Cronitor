using Cronitor.Constants.Scheduling;
using Cronitor.Serialization;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xunit;

namespace Cronitor.Tests
{
    public class ScheduleTests
    {
        [Fact]
        public void EveryMinute()
        {
            var result = Schedule.Cron.EveryMinute();

            Assert.Equal("* * * * *", result.Value);
        }

        [Fact]
        public void EveryHourAtZero()
        {
            var result = Schedule.Cron.EveryHour();

            Assert.Equal("0 * * * *", result.Value);
        }

        [Fact]
        public void EveryHourAtMinute()
        {
            var result = Schedule.Cron.EveryHour(minute: 30);

            Assert.Equal("30 * * * *", result.Value);
        }

        [Fact]
        public void DailyAtMidnight()
        {
            var result = Schedule.Cron.Daily();

            Assert.Equal("0 0 * * *", result.Value);
        }

        [Fact]
        public void DailyAtHour()
        {
            var result = Schedule.Cron.Daily(hour: 3);

            Assert.Equal("0 3 * * *", result.Value);
        }

        [Fact]
        public void DailyAtHourAndMinute()
        {
            var result = Schedule.Cron.Daily(hour: 0, minute: 35);

            Assert.Equal("35 0 * * *", result.Value);
        }

        [Fact]
        public void WeeklyOnDay()
        {
            var result = Schedule.Cron.Weekly(day: 1);

            Assert.Equal("0 0 * * 1", result.Value);
        }

        [Fact]
        public void WeeklyOnDayAtTime()
        {
            var result = Schedule.Cron.Weekly(day: 1, hour: 8, minute: 30);

            Assert.Equal("30 8 * * 1", result.Value);
        }

        [Fact]
        public void MonthlyOnDay()
        {
            var result = Schedule.Cron.Monthly(day: 15);

            Assert.Equal("0 0 15 * *", result.Value);
        }

        [Fact]
        public void MonthlyOnDayAtTime()
        {
            var result = Schedule.Cron.Monthly(day: 1, hour: 6, minute: 15);

            Assert.Equal("15 6 1 * *", result.Value);
        }

        [Fact]
        public void YearlyOnDate()
        {
            var result = Schedule.Cron.Yearly(month: 1, day: 1);

            Assert.Equal("0 0 1 1 *", result.Value);
        }

        [Fact]
        public void YearlyOnDateAtTime()
        {
            var result = Schedule.Cron.Yearly(month: 6, day: 15, hour: 12, minute: 30);

            Assert.Equal("30 12 15 6 *", result.Value);
        }

        [Fact]
        public void RawExpression()
        {
            var result = Schedule.Cron.Expression("0 0 * * *");

            Assert.Equal("0 0 * * *", result.Value);
        }

        [Fact]
        public void EverySeconds()
        {
            var result = Schedule.Every(60).Seconds;

            Assert.Equal("every 60 seconds", result.Value);
        }

        [Fact]
        public void EveryMinutes()
        {
            var result = Schedule.Every(5).Minutes;

            Assert.Equal("every 5 minutes", result.Value);
        }

        [Fact]
        public void EveryHours()
        {
            var result = Schedule.Every(2).Hours;

            Assert.Equal("every 2 hours", result.Value);
        }

        [Fact]
        public void EveryDays()
        {
            var result = Schedule.Every(1).Days;

            Assert.Equal("every 1 days", result.Value);
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

            Assert.Equal("30 3 15 6 1", result.Value);
        }

        [Fact]
        public void CustomCronBuilderDefaults()
        {
            var result = Schedule.Cron.Create()
                .Minute(0)
                .Hour(3)
                .Build();

            Assert.Equal("0 3 * * *", result.Value);
        }

        [Fact]
        public void CustomCronBuilderWithStep()
        {
            var result = Schedule.Cron.Create()
                .MinuteEvery(15)
                .Build();

            Assert.Equal("*/15 * * * *", result.Value);
        }

        [Fact]
        public void CustomCronBuilderWithRange()
        {
            var result = Schedule.Cron.Create()
                .Minute(0)
                .HourRange(9, 17)
                .DayOfWeekRange(1, 5)
                .Build();

            Assert.Equal("0 9-17 * * 1-5", result.Value);
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

        [Fact]
        public void ToStringReturnsValue()
        {
            var expression = Schedule.Cron.Daily(hour: 3);

            Assert.Equal("0 3 * * *", expression.ToString());
        }

        [Fact]
        public void ImplicitStringConversion()
        {
            string result = Schedule.Every(5).Minutes;

            Assert.Equal("every 5 minutes", result);
        }

        [Fact]
        public void ImplicitScheduleExpressionFromString()
        {
            ScheduleExpression expression = "0 0 * * *";

            Assert.Equal("0 0 * * *", expression.Value);
        }

        [Fact]
        public void SerializesAsJsonString()
        {
            var expression = Schedule.Cron.Daily();

            var json = Serializer.Serialize(new { schedule = expression });

            Assert.Equal("{\"schedule\":\"0 0 * * *\"}", json);
        }

        [Fact]
        public void DeserializesFromJsonString()
        {
            var json = "{\"schedule\":\"0 3 * * *\"}";

            var result = Serializer.Deserialize<ScheduleContainer>(json);

            Assert.Equal("0 3 * * *", result.Schedule.Value);
        }

        private class ScheduleContainer
        {
            [JsonPropertyName("schedule")]
            public ScheduleExpression Schedule { get; set; }
        }
    }
}
