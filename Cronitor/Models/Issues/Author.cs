using System.Text.Json.Serialization;

namespace Cronitor.Models.Issues
{
    public class Author
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
