using System;
using System.Collections.Generic;
using System.Linq;

namespace Cronitor.Tests.Helpers
{
    public static class QueryString
    {
        /// <summary>
        /// Parses the query string of <paramref name="uri"/> into unescaped name/value pairs.
        /// </summary>
        public static IReadOnlyDictionary<string, string> Parse(Uri uri)
        {
            return uri.Query
                .TrimStart('?')
                .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(pair => pair.Split(new[] { '=' }, 2))
                .ToDictionary(pair => Uri.UnescapeDataString(pair[0]), pair => Uri.UnescapeDataString(pair[1]));
        }
    }
}
