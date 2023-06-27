using Cronitor.Abstractions;
using Cronitor.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cronitor.Responses.Notifications
{
    public class ListResponse : Response<Template>
    {
        [JsonProperty("templates")]
        public override IEnumerable<Template> Data { get; set; }
    }
}
