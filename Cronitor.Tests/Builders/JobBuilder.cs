using System.Collections.Generic;
using Cronitor.Constants.Scheduling;
using Cronitor.Models.Monitors;

namespace Cronitor.Tests.Builders
{
    public class JobBuilder
    {
        private string _key = "Key";
        private readonly string _realertInterval = "6 hours";
        private readonly int? _failureTolerance = 2;
        private readonly int _graceSeconds = 900;
        private readonly string _group = "Group";
        private readonly string _note = "Note";
        private readonly string _platform = "Platform";
        private ScheduleExpression _schedule = new ScheduleExpression("35 0 * * *");
        private readonly int? _scheduleTolerance = 1;
        private readonly string _timeZone = "Europe/Stockholm";

        private List<string> _assertions = new List<string> { "metric.duration < 30s", "metric.error_count < 5" };
        private List<string>  _notify = new List<string> { "developers" };
        private readonly List<string> _tags = new List<string> { "tag", "attribute" };

        public Job Build()
        {
            return new Job(_key)
            {
                RealertInterval = _realertInterval,
                Assertions = _assertions,
                FailureTolerance = _failureTolerance,
                GraceSeconds = _graceSeconds,
                Group = _group,
                Note = _note,
                Notify = _notify,
                Platform =  _platform,
                Schedule = _schedule,
                ScheduleTolerance = _scheduleTolerance,
                Tags = _tags,
                Timezone = _timeZone
            };
        }

        public JobBuilder Key(string key)
        {
            _key = key;
            return this;
        }

        public JobBuilder Assertions(List<string> assertions)
        {
            _assertions = assertions;
            return this;
        }

        public JobBuilder Notify(List<string> notify)
        {
            _notify = notify;
            return this;
        }

        public JobBuilder Schedule(ScheduleExpression schedule)
        {
            _schedule = schedule;
            return this;
        }
    }
}
