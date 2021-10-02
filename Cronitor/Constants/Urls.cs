using System;

namespace Cronitor.Constants
{
    public static class Urls
    {
        public static Uri ApiUrl = new Uri("https://cronitor.io/api/");

        public static Uri PrimaryBaseUrl = new Uri("https://cronitor.link/p/:apiKey/:key/:command");
        public static Uri FallbackBaseUrl = new Uri("https://cronitor.io/p/:apiKey/:key/:command");
    }
}
