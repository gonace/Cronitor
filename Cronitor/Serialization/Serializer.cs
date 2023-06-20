using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cronitor.Serialization
{
    public static class Serializer
    {
        public static string Serialize(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true,
                        ProcessExtensionDataNames = true,
                        OverrideSpecifiedNames = true
                    }
                },
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }
    }
}
