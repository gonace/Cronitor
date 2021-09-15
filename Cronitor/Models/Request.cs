using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cronitor.Models
{
    public class Request
    {
        /// <summary>
        /// The URL of the resource to monitor.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// The HTTP request method: GET, HEAD, PATCH, POST, PUT
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        /// The request body. Required for PUT, PATCH, and POST requests.
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }
        /// <summary>
        /// The HTTP headers of the request (e.g. User-Agent). Limited to 5120 chars.
        /// </summary>
        [JsonProperty("headers")]
        public dynamic Headers { get; set; }
        /// <summary>
        /// Cookies to set before each request. Limited to 5120 chars.
        /// </summary>
        [JsonProperty("cookies")]
        public dynamic Cookies { get; set; }
        /// <summary>
        /// The timeout enforced when making the request. An alert will be triggered upon timeout.
        /// </summary>
        [JsonProperty("timeout_seconds")]
        public string TimeoutSeconds { get; set; }
        [JsonProperty("regions")]
        public IEnumerable<string> Regions { get; set; }
    }
}
