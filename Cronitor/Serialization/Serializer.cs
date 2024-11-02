using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Serialization
{
    public static class Serializer
    {
        public static string Serialize(object obj)
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            return json;
        }
    }
}
