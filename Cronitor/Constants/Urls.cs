namespace Cronitor.Constants
{
    public static class Urls
    {
        public static string ApiUrl = "https://cronitor.io/api/";

        public static string PrimaryBaseUrl = "https://cronitor.link/p/:apiKey/:key/:command";
        public static string FallbackBaseUrl = "https://cronitor.io/p/:apiKey/:key/:command";
    }
}
