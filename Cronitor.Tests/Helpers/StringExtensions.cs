using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Tests.Helpers
{
    public static class StringExtensions
    {
        public static T Deserialize<T>(this string json)
        {
            var options = new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var o = JsonSerializer.Deserialize<T>(json, options);
            return o;
        }
    }
}