using Cronitor.Exceptions;
using Cronitor.Models;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

public class ApiExceptionTests
{
    [Fact]
    public void Should_set_Message_when_constructed_with_only_a_message()
    {
        const string message = "Test error message";

        var exception = new ApiException(message);

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Should_format_Message_when_constructed_with_a_message_and_status_code()
    {
        const string message = "Test error";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

        var exception = new ApiException(message, statusCode);

        Assert.Equal("Test error (BadRequest)", exception.Message);
    }

    [Fact]
    public void Should_format_Message_when_constructed_with_a_message_and_NotFound()
    {
        const string message = "Resource not found";
        const HttpStatusCode statusCode = HttpStatusCode.NotFound;

        var exception = new ApiException(message, statusCode);

        Assert.Equal("Resource not found (NotFound)", exception.Message);
    }

    [Fact]
    public void Should_use_the_ApiError_message_when_constructed_with_an_ApiError()
    {
        var apiError = new ApiError { Message = "API error message" };

        var exception = new ApiException(apiError);

        Assert.Equal("API error message", exception.Message);
    }

    [Fact]
    public void Should_format_Message_when_constructed_with_an_ApiError_and_status_code()
    {
        var apiError = new ApiError { Message = "Invalid request" };
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

        var exception = new ApiException(apiError, statusCode);

        Assert.Equal("Invalid request (BadRequest)", exception.Message);
    }

    [Fact]
    public void Should_format_Message_when_constructed_with_an_ApiError_and_Unauthorized()
    {
        var apiError = new ApiError { Message = "Unauthorized access" };
        const HttpStatusCode statusCode = HttpStatusCode.Unauthorized;

        var exception = new ApiException(apiError, statusCode);

        Assert.Equal("Unauthorized access (Unauthorized)", exception.Message);
    }

    [Fact]
    public void Should_format_Message_when_constructed_with_an_ApiError_and_InternalServerError()
    {
        var apiError = new ApiError { Message = "Server error" };
        const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

        var exception = new ApiException(apiError, statusCode);

        Assert.Equal("Server error (InternalServerError)", exception.Message);
    }

    [Fact]
    public async Task Should_be_thrown_and_caught()
    {
        const string message = "Test exception";

        var exception = await Assert.ThrowsAsync<ApiException>(() => throw new ApiException(message));

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task Should_be_caught_as_an_Exception()
    {
        const string message = "Test exception";

        var exception = await Assert.ThrowsAsync<ApiException>(() => throw new ApiException(message));

        Assert.IsType<ApiException>(exception);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task Should_be_thrown_and_caught_with_a_status_code()
    {
        const string message = "Test exception";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

        var exception = await Assert.ThrowsAsync<ApiException>(() => throw new ApiException(message, statusCode));

        Assert.Equal("Test exception (BadRequest)", exception.Message);
    }
}
