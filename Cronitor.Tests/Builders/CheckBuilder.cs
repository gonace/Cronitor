using System.Collections.Generic;
using Cronitor.Constants;
using Cronitor.Models.Monitors;

namespace Cronitor.Tests.Builders
{
    public class CheckBuilder
    {
        private string _schedule = "every 60 seconds";
        private string _timezone = "Europe/Stockholm";
        private string _url = "http://www.google.se";

        private List<Region> _regions = new List<Region>
        {
            Region.Bahrain,
            Region.California,
            Region.Dublin,
            Region.Frankfurt,
            Region.Mumbai,
            Region.Ohio,
            Region.SaoPaulo,
            Region.Singapore,
            Region.Stockholm,
            Region.Sydney,
            Region.Virginia
        };

        public Check Build()
        {
            return new Check(new Models.Request(_url, _regions))
            {
                Schedule = _schedule,
                Timezone = _timezone
            };
        }
    }
}