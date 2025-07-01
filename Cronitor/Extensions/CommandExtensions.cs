using Cronitor.Attributes;
using Cronitor.Commands;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cronitor.Extensions
{
    public static class CommandExtensions
    {
        public static string ToQueryString(this Command command)
        {
            var queryStringBuilder = new StringBuilder();
            var type = command.GetType();

            var props = type.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(QueryStringPropertyAttribute)) && (p.GetValue(command, null) != null))
                .ToList();

            if (props.Any())
                queryStringBuilder.Append('?');

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(command, null);
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
