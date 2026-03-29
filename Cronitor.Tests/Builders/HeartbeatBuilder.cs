using Cronitor.Constants.Scheduling;
using Cronitor.Models.Monitors;

namespace Cronitor.Tests.Builders
{
    public class HeartbeatBuilder
    {
        private readonly string _key = "Key";
        private readonly ScheduleExpression _schedule = new ScheduleExpression("every 60 seconds");
        private readonly string _timezone = "Europe/Stockholm";

        public Heartbeat Build()
        {
            return new Heartbeat(_key, _schedule)
            {
                Timezone = _timezone
            };
        }
    }
}
