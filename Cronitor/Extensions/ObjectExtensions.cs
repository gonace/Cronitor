﻿using Cronitor.Attributes;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cronitor.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            var queryStringBuilder = new StringBuilder();
            var objType = obj.GetType();

            var props = objType.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(QueryStringPropertyAttribute)) && (p.GetValue(obj, null) != null))
                .ToList();

            if (props.Any())
                queryStringBuilder.Append('?');

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(obj, null);
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
