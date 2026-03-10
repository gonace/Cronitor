using System;

namespace Cronitor.Extensions
{
    public static class ArgumentHelper
    {
        public static void ThrowIfNullOrWhiteSpace(string key)
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