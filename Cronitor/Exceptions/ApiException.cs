using Cronitor.Models;
using System;
using System.Net;

namespace Cronitor.Exceptions;

public class ApiException : Exception
{
    public ApiException(string message)
        : base(message)
    {
    }

    public ApiException(string message, HttpStatusCode statusCode)
        : base($"{message} ({statusCode})")
    {
        StatusCode = statusCode;
    }

    public ApiException(ApiError model)
        : base(model.Message)
    {
    }

    public ApiException(ApiError model, HttpStatusCode statusCode)
        : base($"{model.Message} ({statusCode})")
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// An error Cronitor answered with, keeping its status code and response body.
    /// </summary>
    public ApiException(ApiError model, HttpStatusCode statusCode, string response)
        : this(model, statusCode)
    {
        Response = response;
    }

    /// <summary>
    /// The HTTP status code Cronitor answered with, if the error came from a response.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// The response body Cronitor answered with, if the error came from a response. A rejected bulk update names each
    /// rejected monitor here, with Cronitor's reasons, such as <c>{"nightly-backup": {"schedules": ["Invalid schedule expression: …"]}}</c>.
    /// </summary>
    public string Response { get; }
}
