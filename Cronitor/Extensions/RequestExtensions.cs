using Cronitor.Abstractions;

namespace Cronitor.Extensions
{
    public static class RequestExtensions
    {
        public static string ToQueryString(this BaseRequest request)
        {
            return request.ToQueryString<BaseRequest>();
        }
    }
}