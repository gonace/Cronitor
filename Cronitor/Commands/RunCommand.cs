using System.Net.Http;

namespace Cronitor.Commands
{
    public class RunCommand : Command
    {
        public override string Endpoint => "run";

        public RunCommand()
        {
            Method = HttpMethod.Get;
        }
    }
}