using System;
using System.Linq;
using System.Text;
using Cronitor.Attributes;

namespace Cronitor.Extensions
{
    public static class ObjectExtensions
    {
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
                    value = attribute.Lower ? Uri.EscapeDataString(value.ToString().ToLower()) : Uri.EscapeDataString(value.ToString());

                    // Check's if this is the last property, if so, don't add an '&'
                    queryStringBuilder.Append(props.IndexOf(prop) != (props.Count - 1)
                        ? $"{Uri.EscapeDataString(name)}={value}&"
                        : $"{Uri.EscapeDataString(name)}={value}");
                }
            }

            return queryStringBuilder.ToString();
        }
    }
}
