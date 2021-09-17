using System;
using System.Linq;
using System.Text;
using Cronitor.Attributes;

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

        public static string ToQueryString(this object obj)
        {
            var queryStringBuilder = new StringBuilder("");
            var objType = obj.GetType();

            var props = objType.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(QueryStringAttribute)) && (p.GetValue(obj, null) != null))
                .ToList();

            if (props.Any())
                queryStringBuilder.Append("?");

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(obj, null);
                var attribute = (QueryStringAttribute)Attribute.GetCustomAttribute(prop, typeof(QueryStringAttribute));

                if (value != null)
                {
                    name = attribute.PropertyName ?? name.ToLower();
                    // Check's if this is the last property, if so, don't add an '&'
                    queryStringBuilder.Append(props.IndexOf(prop) != (props.Count - 1)
                        ? $"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value.ToString())}&"
                        : $"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value.ToString())}");
                }
            }

            return queryStringBuilder.ToString();
        }
    }
}
