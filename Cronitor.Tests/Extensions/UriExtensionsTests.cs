using System;
using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests.Extensions
{
    public class UriExtensionsTests
    {
        [Fact]
        public void ChangesSchemeToHttp()
        {
            var uri = new Uri("https://www.google.se/");

            Assert.Equal("http://www.google.se/", uri.AsHttp().ToString());
        }

        [Fact]
        public void ChangesSchemeToHttps()
        {
            var uri = new Uri("http://www.google.se/");

            Assert.Equal("https://www.google.se/", uri.AsHttps().ToString());
        }
    }
}
