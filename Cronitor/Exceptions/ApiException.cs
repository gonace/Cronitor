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

        public ApiException(Models.ApiException model)
            : base(model.Message)
        {
        }

        public ApiException(Models.ApiException model, HttpStatusCode statusCode)
            : base($"{model.Message} ({statusCode})")
        {
        }
    }
}
