﻿using System.Collections.Generic;
using Cronitor.Models.Monitors;

namespace Cronitor.Tests.Builders
{
    public class JobBuilder
    {
        private string _alertInterval = "6 hours";
        private int? _failureTolerance = 2;
        private int _graceSeconds = 900;
        private string _group = "Group";
        private string _note = "Note";
        private string _platform = "Platform";
        private string _schedule = "35 0 * * *";
        private int? _scheduleTolerance = 1;
        private string _timeZone = "Europe/Stockholm";

        private List<string> _assertions = new List<string> { "metric.duration < 30s", "metric.error_count < 5" };
        private List<string>  _notify = new List<string> { "developers" };
        private List<string> _tags = new List<string> { "tag", "attribute" };

        public Job Build()
        {
            return new Job
            {
                AlertInterval = _alertInterval,
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

        public JobBuilder Schedule(string schedule)
        {
            _schedule = schedule;
            return this;
        }
    }
}