using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Cronitor.Attributes;

namespace Cronitor.Extensions
{
    internal static class ObjectExtensions
    {
        public static string ToQueryString<T>(this T @object)
        {
            var queryStringBuilder = new StringBuilder();
            var type = @object.GetType();

            var props = type.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(QueryStringPropertyAttribute)) && (p.GetValue(@object, null) != null))
                .ToList();

            if (props.Any())
                queryStringBuilder.Append('?');

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(@object, null);
                var attribute = (QueryStringPropertyAttribute)Attribute.GetCustomAttribute(prop, typeof(QueryStringPropertyAttribute));

                if (value != null)
                {
                    name = attribute?.PropertyName ?? name.ToLower(CultureInfo.CurrentCulture);
                    value = attribute != null && attribute.Lower
                        ? value.ToString()?.ToLower(CultureInfo.CurrentCulture)
                        : value.ToString();

                    // Check's if this is the last property, if so, don't add an '&'
                    queryStringBuilder.Append(props.IndexOf(prop) != (props.Count - 1)
                        ? $"{name}={value}&"
                        : $"{name}={value}");
                }
            }

            return queryStringBuilder.ToString();
        }
    }
}