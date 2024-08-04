using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses.Notifications
{
    public class ListResponse : BaseResponse<Template>
    {
        [JsonPropertyName("templates")]
        public override IEnumerable<Template> Items { get; set; }
    }
}
