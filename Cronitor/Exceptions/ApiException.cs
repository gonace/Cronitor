using Cronitor.Models;
using System;
using System.Net;

namespace Cronitor.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message)
            : base(message)
        {
        }

        public ApiException(string message, HttpStatusCode statusCode)
            : base($"{message} ({statusCode})")
        {
        }

        public ApiException(ApiError model)
            : base(model.Message)
        {
        }

        public ApiException(ApiError model, HttpStatusCode statusCode)
            : base($"{model.Message} ({statusCode})")
        {
        }
    }
}
