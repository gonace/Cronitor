using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class ApiException
    {
        [JsonProperty("detail")]
        public string Message { get; set; }
    }
}
