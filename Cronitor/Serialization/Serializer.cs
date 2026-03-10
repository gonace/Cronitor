using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Serialization
{
    public static class Serializer
    {
        public static string Serialize(object obj)
        {
            var json = JsonSerializer.Serialize(obj, Options);

            return json;
        }

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            IgnoreReadOnlyProperties = true
        };
    }
}
