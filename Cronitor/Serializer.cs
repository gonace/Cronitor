using Cronitor.Attributes;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Cronitor;

internal static class Serializer
{
    public static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, Options);

    public static string Serialize(object obj) =>
        JsonSerializer.Serialize(obj, Options);

    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        Converters = { MonitorKeyConverter.ForCronitorResponses(), new MonitorConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        IgnoreReadOnlyProperties = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { SkipResponseOnlyPropertiesWhenWriting } },
        WriteIndented = false
    };

    /// <summary>
    /// Properties marked [ResponseOnly], such as a monitor's passing or created, are set by Cronitor.
    /// They are read from responses but never sent in requests.
    /// </summary>
    private static void SkipResponseOnlyPropertiesWhenWriting(JsonTypeInfo typeInfo)
    {
        foreach (var property in typeInfo.Properties)
        {
            if (property.AttributeProvider?.IsDefined(typeof(ResponseOnlyAttribute), true) == true)
            {
                property.ShouldSerialize = (_, _) => false;
            }
        }
    }
}
