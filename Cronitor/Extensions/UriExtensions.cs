using System;

namespace Cronitor.Extensions
{
    public static class UriExtensions
    {
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
