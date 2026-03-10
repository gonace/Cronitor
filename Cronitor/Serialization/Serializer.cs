using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Serialization
{
    public static class Serializer
    {
        public static T Deserialize<T>(string json) => 
            JsonSerializer.Deserialize<T>(json, Options);

        public static string Serialize(object obj) =>
            JsonSerializer.Serialize(obj, Options);

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreReadOnlyProperties = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = false
        };
    }
}
