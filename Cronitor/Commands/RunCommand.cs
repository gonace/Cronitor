using Cronitor.Constants;
using System.Net.Http;

namespace Cronitor.Commands
{
    public class RunCommand : Command
    {
        public override string Endpoint => "run";

        public RunCommand()
            : base(Urls.TelemetryBaseUrl)
        {
            Method = HttpMethod.Get;
        }
    }
}