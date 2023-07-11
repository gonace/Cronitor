using System;

namespace Cronitor.Exceptions
{
    public class NotConfiguredException : Exception
    {
        public NotConfiguredException()
            : base("You need to run .Configure(...) before accessing properties on Cronitor.")
        {
        }
    }
}
