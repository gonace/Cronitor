using System.Net.Http;

namespace Cronitor.Commands
{
    public class TickCommand : Command
    {
        public override string Endpoint => "tick";

        public TickCommand()
        {
            Method = HttpMethod.Get;
        }
    }
}