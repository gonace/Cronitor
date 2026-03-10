using Cronitor.Abstractions;
using Cronitor.Constants;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Cronitor.Tests
{
    // Concrete implementation for testing BaseRequest
    internal class TestRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "test-endpoint";
    }

    public class BaseRequestTests
    {
        [Fact]
        public void SetContent_ShouldSetContentProperty()
        {
            var request = new TestRequest();
            var content = new StringContent("test content", Encoding.UTF8, "application/json");

            request.SetContent(content);

            Assert.NotNull(request.Content);
            Assert.Same(content, request.Content);
        }

        [Fact]
        public void SetContent_ShouldReturnThis_ForFluentAPI()
        {
            var request = new TestRequest();
            var content = new StringContent("test content", Encoding.UTF8, "application/json");

            var result = request.SetContent(content);

            Assert.NotNull(result);
            Assert.Same(request, result);
        }

        [Fact]
        public void SetContent_CanBeChained()
        {
            var request = new TestRequest();
            var content = new StringContent("test content", Encoding.UTF8, "application/json");

            var result = request.SetContent(content).SetContent(content);

            Assert.NotNull(result);
            Assert.Same(request, result);
        }

        [Fact]
        public void ToUri_ShouldCombineBaseUrlAndEndpoint()
        {
            var request = new TestRequest();

            var uri = request.ToUri();

            Assert.NotNull(uri);
            Assert.Contains("test-endpoint", uri.ToString());
            Assert.StartsWith(Urls.DefaultApiUrl.ToString(), uri.ToString());
        }

        [Fact]
        public void ToUrl_ShouldReturnStringUri()
        {
            var request = new TestRequest();

            var url = request.ToUrl();

            Assert.NotNull(url);
            Assert.IsType<string>(url);
            Assert.Contains("test-endpoint", url);
            Assert.StartsWith(Urls.DefaultApiUrl.ToString(), url);
        }

        [Fact]
        public void ToUrl_ShouldEqualToUriToString()
        {
            var request = new TestRequest();

            var url = request.ToUrl();
            var uriString = request.ToUri().ToString();

            Assert.Equal(uriString, url);
        }

        [Fact]
        public void Method_DefaultsToGet()
        {
            var request = new TestRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void Method_CanBeSet()
        {
            var request = new TestRequest
            {
                Method = HttpMethod.Post
            };

            Assert.Equal(HttpMethod.Post, request.Method);
        }

        [Fact]
        public void Endpoint_CanBeSet()
        {
            var request = new TestRequest
            {
                Endpoint = "custom-endpoint"
            };

            Assert.Equal("custom-endpoint", request.Endpoint);
        }

        [Fact]
        public void Content_DefaultsToNull()
        {
            var request = new TestRequest();

            Assert.Null(request.Content);
        }

        [Fact]
        public void Content_CanBeSetDirectly()
        {
            var request = new TestRequest();
            var content = new StringContent("test", Encoding.UTF8, "application/json");

            request.Content = content;

            Assert.NotNull(request.Content);
            Assert.Same(content, request.Content);
        }
    }
}
