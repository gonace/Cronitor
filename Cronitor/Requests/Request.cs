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

        public virtual string ToUrl()
        {
            return Endpoint;
        }
    }
}
