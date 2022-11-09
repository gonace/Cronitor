using Cronitor.Constants;
using Cronitor.Extensions;
using System;
using System.Net.Http;

namespace Cronitor.Requests
{
    public abstract class Request
    {
        public HttpContent Content { get; set; }
        public virtual HttpMethod Method { get; set; } = HttpMethod.Get;
        public abstract string Endpoint { get; set; }


        public Request SetContent(HttpContent content)
        {
            Content = content;

            return this;
        }

        public virtual Uri ToUri()
        {
            return new Uri(Urls.ApiUrl.ToString()).Combine(Endpoint);
        }

        public string ToUrl()
        {
            return ToUri().ToString();
        }
    }
}
