namespace Cronitor.Constants
{
    public static class Urls
    {
        public static string PrimaryApiUrl = "https://cronitor.io/api/";
        public static string FallbackApiUrl = "https://cronitor.link/api/";

        public static string PrimaryBaseUrl = "https://cronitor.link/p/:apiKey/:key/:command";
        public static string FallbackBaseUrl = "https://cronitor.io/p/:apiKey/:key/:command";

        //public static readonly Url PrimaryBaseUrl = new Url("https://cronitor.link/p/%s/%s");
        //public static readonly Url FallbackBaseUrl = new Url("https://cronitor.io/p/%s/%s");

        //public string Address { get; set; }

        //private Url(string address)
        //{
        //    Address = address;
        //}
    }
}
