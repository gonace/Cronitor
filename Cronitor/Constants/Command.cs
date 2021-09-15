using System.Net.Http;

namespace Cronitor.Constants
{
    public class Command
    {
        public static readonly Command Run = new Command(HttpMethod.Get, "run");
        public static readonly Command Complete = new Command(HttpMethod.Get, "complete");
        public static readonly Command Pause = new Command(HttpMethod.Get, "pause");
        public static readonly Command Unpause = new Command(HttpMethod.Get, "pause");
        public static readonly Command Fail = new Command(HttpMethod.Get, "fail");
        public static readonly Command Tick = new Command(HttpMethod.Get, "tick");

        public HttpContent Content { get; set; }
        public HttpMethod Method { get; set; }
        public string Endpoint { get; }

        private Command(HttpMethod method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
        }

        public Command SetContent(HttpContent content)
        {
            Content = content;

            return this;
        }

        public Command SetMethod(HttpMethod method)
        {
            Method = method;

            return this;
        }

        public override string ToString()
        {
            return Endpoint;
        }
    }
}
