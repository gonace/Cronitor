using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
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
        public void ShouldConstructWithUriApiKey()
        {
            var httpClient = new HttpClient(Urls.DefaultApiUrl, ApiKey);

            Assert.NotNull(httpClient);
        }

        [Fact]
        public void ShouldConvertHttpToHttpsWhenConstructingWithApiKey()
        {
            var httpUri = new Uri("http://cronitor.io/api/");
            var httpClient = new HttpClient(httpUri, ApiKey);
            var apiUriField = typeof(HttpClient).GetField("_apiUri", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actualUri = (Uri)apiUriField.GetValue(httpClient);

            Assert.NotNull(actualUri);
            Assert.Equal("https", actualUri.Scheme);
            Assert.Equal("cronitor.io", actualUri.Host);
        }
    }
}
