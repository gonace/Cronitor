using Cronitor.Exceptions;
using Cronitor.Models;
using Cronitor.Tests.Helpers;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Cronitor.Tests;

public class HttpClientErrorHandlingTests : BaseTest
{
    private readonly JsonSerializerOptions _options;

    public HttpClientErrorHandlingTests()
    {
        _options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }

    [Fact]
    public void Should_throw_ApiException_for_BadRequest()
    {
        var apiError = new ApiError { Message = "Bad request error" };
        var json = JsonSerializer.Serialize(apiError);

        var deserializedError = JsonSerializer.Deserialize<ApiError>(json, _options);
        Assert.Equal("Bad request error", deserializedError.Message);

        var exception = new ApiException(deserializedError, HttpStatusCode.BadRequest);
        Assert.Equal("Bad request error (BadRequest)", exception.Message);
    }

    [Fact]
    public void Should_throw_ApiException_for_Unauthorized()
    {
        var apiError = new ApiError { Message = "Unauthorized access" };
        var json = JsonSerializer.Serialize(apiError);

        var deserializedError = JsonSerializer.Deserialize<ApiError>(json, _options);
        var exception = new ApiException(deserializedError, HttpStatusCode.Unauthorized);

        Assert.Equal("Unauthorized access (Unauthorized)", exception.Message);
    }

    [Fact]
    public void Should_throw_ApiException_for_NotFound()
    {
        var apiError = new ApiError { Message = "Resource not found" };
        var json = JsonSerializer.Serialize(apiError);

        var deserializedError = JsonSerializer.Deserialize<ApiError>(json, _options);
        var exception = new ApiException(deserializedError, HttpStatusCode.NotFound);

        Assert.Equal("Resource not found (NotFound)", exception.Message);
    }

    [Fact]
    public void Should_throw_ApiException_for_InternalServerError()
    {
        var apiError = new ApiError { Message = "Internal server error" };
        var json = JsonSerializer.Serialize(apiError);

        var deserializedError = JsonSerializer.Deserialize<ApiError>(json, _options);
        var exception = new ApiException(deserializedError, HttpStatusCode.InternalServerError);

        Assert.Equal("Internal server error (InternalServerError)", exception.Message);
    }

    [Fact]
    public void Should_deserialize_ApiError_from_json()
    {
        const string json = "{\"detail\":\"Test error message\"}";

        var apiError = JsonSerializer.Deserialize<ApiError>(json, _options);

        Assert.NotNull(apiError);
        Assert.Equal("Test error message", apiError.Message);
    }

    [Fact]
    public void Should_handle_a_null_message_in_ApiError()
    {
        const string json = "{}";

        var apiError = JsonSerializer.Deserialize<ApiError>(json, _options);

        Assert.NotNull(apiError);
        Assert.Null(apiError.Message);
    }

    [Fact]
    public void Should_format_the_HttpStatusCode_in_the_exception_message()
    {
        var statusCodes = new[]
        {
            (HttpStatusCode.BadRequest, "BadRequest"),
            (HttpStatusCode.Unauthorized, "Unauthorized"),
            (HttpStatusCode.Forbidden, "Forbidden"),
            (HttpStatusCode.NotFound, "NotFound"),
            (HttpStatusCode.InternalServerError, "InternalServerError"),
            (HttpStatusCode.ServiceUnavailable, "ServiceUnavailable")
        };

        foreach (var (statusCode, expectedName) in statusCodes)
        {
            var exception = new ApiException("Error", statusCode);
            Assert.Contains(expectedName, exception.Message);
        }
    }
}
