using System;

namespace Cronitor.Extensions
{
    internal static class ArgumentHelper
    {
        internal static void ThrowIfNullOrWhiteSpace(string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}