using Cronitor.Constants;
using System.Net.Http;

namespace Cronitor.Commands
{
    public class TickCommand : Command
    {
        public override string Endpoint => "tick";

        public TickCommand()
            : base(Urls.TelemetryBaseUrl)
        {
            Method = HttpMethod.Get;
        }
    }
}