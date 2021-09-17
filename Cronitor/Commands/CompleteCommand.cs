using System.Net.Http;

namespace Cronitor.Commands
{
    public class CompleteCommand : Command
    {
        public override string Endpoint => "complete";

        public CompleteCommand()
        {
            Method = HttpMethod.Get;
        }
    }
}