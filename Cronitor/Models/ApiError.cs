using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class ApiError
    {
        [JsonPropertyName("detail")]
        public string Message { get; set; }
    }
}
