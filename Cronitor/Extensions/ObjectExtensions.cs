using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Cronitor.Attributes;

namespace Cronitor.Extensions;

internal static class ObjectExtensions
{
    public static string ToQueryString<T>(this T @object)
    {
        var pairs = @object.GetType().GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(QueryStringPropertyAttribute)))
            .SelectMany(p => ToQueryStringPairs(@object, p))
            .ToList();

        return pairs.Any()
            ? "?" + string.Join("&", pairs)
            : string.Empty;
    }

    /// <summary>
    /// Returns one name=value pair per value, with the value URL-encoded; a collection repeats the name for every item.
    /// </summary>
    private static IEnumerable<string> ToQueryStringPairs(object @object, PropertyInfo property)
    {
        var value = property.GetValue(@object, null);
        if (value == null)
        {
            yield break;
        }

        var attribute = (QueryStringPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(QueryStringPropertyAttribute));
        var name = attribute.PropertyName ?? property.Name.ToLower(CultureInfo.CurrentCulture);

        foreach (var item in ToItems(value))
        {
            var text = attribute.Lower
                ? item.ToString()?.ToLower(CultureInfo.CurrentCulture)
                : item.ToString();

            yield return $"{name}={Uri.EscapeDataString(text ?? string.Empty)}";
        }
    }

    /// <summary>
    /// A string is a single value even though it is enumerable; other collections give one item per element.
    /// </summary>
    private static IEnumerable<object> ToItems(object value)
    {
        if (value is string)
        {
            return new[] { value };
        }

        if (value is IEnumerable collection)
        {
            return collection.Cast<object>();
        }

        return new[] { value };
    }
}
