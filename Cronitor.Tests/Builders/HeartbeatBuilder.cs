using Cronitor.Models.Monitors;

namespace Cronitor.Tests.Builders
{
    public class HeartbeatBuilder
    {
        private string _schedule = "every 60 seconds";
        private string _timezone = "Europe/Stockholm";

        public Heartbeat Build()
        {
            return new Heartbeat(_schedule)
            {
                Timezone = _timezone
            };
        }
    }
}