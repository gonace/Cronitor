using System;
using System.Runtime.CompilerServices;

namespace Cronitor.Extensions;

internal static class ArgumentHelper
{
    internal static void ThrowIfNullOrWhiteSpace(string value, [CallerArgumentExpression(nameof(value))] string paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
