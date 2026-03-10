using System;

namespace Cronitor.Extensions
{
    internal static class ArgumentHelper
    {
        internal static void ThrowIfNullOrWhiteSpace(string key)
        {
#if NET8_0_OR_GREATER
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
#else
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value cannot be null or whitespace", nameof(key));
            }
#endif
        }
    }
}