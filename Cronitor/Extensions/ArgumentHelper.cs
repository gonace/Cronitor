using System;
using System.Runtime.CompilerServices;

namespace Cronitor.Extensions
{
    internal static class ArgumentHelper
    {
#if NET8_0_OR_GREATER
        internal static void ThrowIfNullOrWhiteSpace(string value, [CallerArgumentExpression(nameof(value))] string paramName = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        }
#else
        internal static void ThrowIfNullOrWhiteSpace(string value, string paramName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null or whitespace", paramName);
            }
        }
#endif
    }
}