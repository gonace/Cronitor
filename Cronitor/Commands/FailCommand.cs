using Cronitor.Constants;
using System.Net.Http;

namespace Cronitor.Commands
{
    public class FailCommand : Command
    {
        public override string Endpoint => "fail";

        public FailCommand()
            : base(Urls.TelemetryBaseUrl)
        {
            Method = HttpMethod.Get;
        }
    }
}