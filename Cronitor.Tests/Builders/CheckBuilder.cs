using System.Collections.Generic;
using Cronitor.Constants;
using Cronitor.Models.Monitors;

namespace Cronitor.Tests.Builders
{
    public class CheckBuilder
    {
        private readonly string _key = "Key";
        private readonly string _schedule = "every 60 seconds";
        private readonly string _timezone = "Europe/Stockholm";
        private readonly string _url = "https://www.google.se";

        private readonly List<Region> _regions = new List<Region>
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
            return new Check(_key, new Models.Request(_url, _regions))
            {
                Schedule = _schedule,
                Timezone = _timezone
            };
        }
    }
}