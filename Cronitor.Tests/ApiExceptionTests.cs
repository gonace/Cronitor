using Cronitor.Exceptions;
using Cronitor.Models;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests
{
    public class ApiExceptionTests
    {
        [Fact]
        public void Constructor_WithMessageOnly_ShouldSetMessage()
        {
            const string message = "Test error message";

            var exception = new ApiException(message);

            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void Constructor_WithMessageAndStatusCode_ShouldFormatMessage()
        {
            const string message = "Test error";
            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            var exception = new ApiException(message, statusCode);

            Assert.Equal("Test error (BadRequest)", exception.Message);
        }

        [Fact]
        public void Constructor_WithMessageAndStatusCode_NotFound_ShouldFormatMessage()
        {
            const string message = "Resource not found";
            const HttpStatusCode statusCode = HttpStatusCode.NotFound;

            var exception = new ApiException(message, statusCode);

            Assert.Equal("Resource not found (NotFound)", exception.Message);
        }

        [Fact]
        public void Constructor_WithApiError_ShouldUseErrorMessage()
        {
            var apiError = new ApiError { Message = "API error message" };

            var exception = new ApiException(apiError);

            Assert.Equal("API error message", exception.Message);
        }

        [Fact]
        public void Constructor_WithApiErrorAndStatusCode_ShouldFormatMessage()
        {
            var apiError = new ApiError { Message = "Invalid request" };
            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            var exception = new ApiException(apiError, statusCode);

            Assert.Equal("Invalid request (BadRequest)", exception.Message);
        }

        [Fact]
        public void Constructor_WithApiErrorAndStatusCode_Unauthorized_ShouldFormatMessage()
        {
            var apiError = new ApiError { Message = "Unauthorized access" };
            const HttpStatusCode statusCode = HttpStatusCode.Unauthorized;

            var exception = new ApiException(apiError, statusCode);

            Assert.Equal("Unauthorized access (Unauthorized)", exception.Message);
        }

        [Fact]
        public void Constructor_WithApiErrorAndStatusCode_InternalServerError_ShouldFormatMessage()
        {
            var apiError = new ApiError { Message = "Server error" };
            const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            var exception = new ApiException(apiError, statusCode);

            Assert.Equal("Server error (InternalServerError)", exception.Message);
        }

        [Fact]
        public async Task CanBeThrownAndCaught()
        {
            const string message = "Test exception";

            var exception = await Assert.ThrowsAsync<ApiException>(() => throw new ApiException(message));

            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CanBeCaughtAsException()
        {
            const string message = "Test exception";

            var exception = await Assert.ThrowsAsync<ApiException>(() => throw new ApiException(message));

            Assert.IsType<ApiException>(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task WithStatusCode_CanBeThrownAndCaught()
        {
            const string message = "Test exception";
            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            var exception = await Assert.ThrowsAsync<ApiException>(() => throw new ApiException(message, statusCode));

            Assert.Equal("Test exception (BadRequest)", exception.Message);
        }
    }
}
