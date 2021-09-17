using System.Net.Http;

namespace Cronitor.Commands
{
    public class FailCommand : Command
    {
        public override string Endpoint => "fail";

        public FailCommand()
        {
            Method = HttpMethod.Get;
        }
    }
}