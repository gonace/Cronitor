using Cronitor.Constants;
using System.Net.Http;

namespace Cronitor.Commands
{
    public class CompleteCommand : Command
    {
        public override string Endpoint => "complete";

        public CompleteCommand()
            : base(Urls.TelemetryBaseUrl)
        {
            Method = HttpMethod.Get;
        }
    }
}