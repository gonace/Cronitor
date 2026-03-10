using Cronitor.Commands;

namespace Cronitor.Extensions
{
    public static class CommandExtensions
    {
        public static string ToQueryString(this Command command)
        {
            return command.ToQueryString<Command>();
        }
    }
}
