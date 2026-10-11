using Cronitor.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class Request
    {
        /// <summary>
        /// The URL of the resource to monitor.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("url")]
        public string Url { get; protected set; }
        /// <summary>
        /// The HTTP request method: GET, HEAD, PATCH, POST, PUT
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; } = "GET";
        /// <summary>
        /// The request body. Required for PUT, PATCH, and POST requests.
        /// </summary>
        [JsonPropertyName("body")]
        public string Body { get; set; }
        /// <summary>
        /// The HTTP headers of the request (e.g. User-Agent). Limited to 5120 chars.
        /// </summary>
        [JsonPropertyName("headers")]
        public dynamic Headers { get; set; }
        /// <summary>
        /// Cookies to set before each request. Limited to 5120 chars.
        /// </summary>
        [JsonPropertyName("cookies")]
        public dynamic Cookies { get; set; }
        /// <summary>
        /// The timeout enforced when making the request. An alert will be triggered upon timeout.
        /// </summary>
        [JsonPropertyName("timeout_seconds")]
        public int TimeoutSeconds { get; set; } = 10;
        /// <summary>
        /// Whether redirects are followed. Cronitor's default applies when not set.
        /// </summary>
        [JsonPropertyName("follow_redirects")]
        public bool? FollowRedirects { get; set; }
        /// <summary>
        /// Whether the SSL certificate is verified. Cronitor's default applies when not set.
        /// </summary>
        [JsonPropertyName("verify_ssl")]
        public bool? VerifySsl { get; set; }
        [JsonInclude]
        [JsonPropertyName("regions")]
        public IEnumerable<string> Regions { get; protected set; }

        [JsonConstructor]
        private Request()
        {
        }

        public Request(string url, IEnumerable<Region> regions)
        {
            Url = url;
            Regions = regions.Select(x => x.Value);
        }
    }
}
