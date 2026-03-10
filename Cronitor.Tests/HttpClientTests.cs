using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Cronitor.Tests
{
    public class HttpClientTests : BaseTest
    {
        [Fact]
        public void ShouldConstructWithNoParameters()
        {
            var httpClient = new HttpClient();

            Assert.NotNull(httpClient);
        }

        [Fact]
        public void ShouldConstructWithUriAndSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            var httpClient = new HttpClient(Urls.DefaultApiUrl, options);

            Assert.NotNull(httpClient);
        }

        [Fact]
        public void ShouldConstructWithUriApiKeyAndSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            var httpClient = new HttpClient(Urls.DefaultApiUrl, ApiKey, options);

            Assert.NotNull(httpClient);
        }

        [Fact]
        public void ShouldConvertHttpToHttpsWhenConstructingWithApiKey()
        {
            var httpUri = new Uri("http://cronitor.io/api/");
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            var httpClient = new HttpClient(httpUri, ApiKey, options);
            var apiUriField = typeof(HttpClient).GetField("_apiUri", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actualUri = (Uri)apiUriField.GetValue(httpClient);

            Assert.NotNull(actualUri);
            Assert.Equal("https", actualUri.Scheme);
            Assert.Equal("cronitor.io", actualUri.Host);
        }
    }
}
