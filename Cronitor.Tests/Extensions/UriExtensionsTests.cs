using System;
using System.Collections.Generic;
using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests.Extensions
{
    public class UriExtensionsTests
    {
        [Fact]
        public void ShouldBuildUri()
        {
            var uri = new Uri("https://www.google.se/:foo/:bar");
            var dictionary = new Dictionary<string, string>
            {
                { ":foo", "bar" },
                { ":bar", "foo" }
            };

            const string expected = "https://www.google.se/bar/foo";
            var actual = uri.Build(dictionary).ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldAddQueryString()
        {
            var uri = new Uri("https://www.google.se");
            const string qs = "?host=127.0.0.1&message=Lorem ipsum&env=Production&series=3de5db91-9c02-4e95-b8a9-9a2442702336&metric=count:9.99";

            var expected = $"https://www.google.se/{qs}";
            var actual = uri.AddQueryString(qs).ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCombineUriWithEndpoint()
        {
            var uri = new Uri("https://www.google.se/");

            var expected = "https://www.google.se/foo/bar";
            var actual = uri.Combine("foo/bar").ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldChangeSchemeToHttp()
        {
            var uri = new Uri("https://www.google.se/");

            const string expected = "http://www.google.se/";
            var actual = uri.AsHttp().ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldChangeSchemeToHttps()
        {
            var uri = new Uri("http://www.google.se/");

            const string expected = "https://www.google.se/";
            var actual = uri.AsHttps().ToString();

            Assert.Equal(expected, actual);
        }
    }
}
