using System;
using System.Collections.Generic;

namespace Cronitor.Extensions
{
    public static class UriExtensions
    {
        public static Uri Build(this Uri uri, IDictionary<string, string> dictionary)
        {
            var address = uri.ToString();

            foreach (var kv in dictionary)
            {
                address = address.Replace(kv.Key, kv.Value);
            }

            return new Uri(address);
        }

        public static Uri Combine(this Uri uri, string endpoint)
        {
            var address = $"{uri}/{endpoint}";

            return new Uri(address);
        }

        public static Uri AsHttp(this Uri uri)
        {
            return new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttp,
                Port = -1
            }.Uri;
        }

        public static Uri AsHttps(this Uri uri)
        {
            return new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1
            }.Uri;
        }
    }
}
