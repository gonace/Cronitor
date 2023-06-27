using System;
using System.Text;

namespace Cronitor.Extensions
{
    public static class StringExtensions
    {
        public static string AddQueryString(this string @string, string queryString)
        {
            return @string + queryString;
        }

        public static string Base64Encode(this string @string)
        {
            var bytes = Encoding.ASCII.GetBytes(@string);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(this string @string)
        {
            var bytes = Convert.FromBase64String(@string);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
