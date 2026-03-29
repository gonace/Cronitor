namespace Cronitor.Constants.Scheduling
{
    public class IntervalBuilder
    {
        private readonly int _value;

        public IntervalBuilder(int value)
        {
            _value = value;
        }

        public ScheduleExpression Seconds => new ScheduleExpression($"every {_value} seconds");
        public ScheduleExpression Minutes => new ScheduleExpression($"every {_value} minutes");
        public ScheduleExpression Hours => new ScheduleExpression($"every {_value} hours");
        public ScheduleExpression Days => new ScheduleExpression($"every {_value} days");
    }
}
