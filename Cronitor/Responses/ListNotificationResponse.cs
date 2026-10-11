using Cronitor.Abstractions;
using Cronitor.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Responses;

public class ListNotificationResponse : BaseResponse<Notification>
{
    [JsonPropertyName("templates")]
    public override IEnumerable<Notification> Items { get; set; }
    [JsonPropertyName("total_template_count")]
    public override int Total { get; set; }
}
