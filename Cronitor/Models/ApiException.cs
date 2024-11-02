using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class ApiException
    {
        [JsonPropertyName("detail")]
        public string Message { get; set; }
    }
}
