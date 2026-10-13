using Cronitor.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Cronitor.Tests;

public class UriExtensionsTests
{
    [Fact]
    public void Should_build_the_uri()
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

    [Theory]
    [InlineData("a?b", "a%3Fb")]
    [InlineData("a#b", "a%23b")]
    [InlineData("a b", "a%20b")]
    [InlineData("a%b", "a%25b")]
    [InlineData("a/b", "a%2Fb")]
    [InlineData("åäö", "%C3%A5%C3%A4%C3%B6")]
    public void Should_escape_the_values_so_each_stays_one_path_segment(string value, string escaped)
    {
        var uri = new Uri("https://cronitor.link/p/:key/run");
        var dictionary = new Dictionary<string, string> { { ":key", value } };

        var actual = uri.Build(dictionary).AbsoluteUri;

        Assert.Equal($"https://cronitor.link/p/{escaped}/run", actual);
    }

    [Fact]
    public void Should_add_a_query_string()
    {
        var uri = new Uri("https://www.google.se");
        const string qs = "?host=127.0.0.1&message=Lorem ipsum&env=Production&series=3de5db91-9c02-4e95-b8a9-9a2442702336&metric=count:9.99";

        var expected = $"https://www.google.se/{qs}";
        var actual = uri.AddQueryString(qs).ToString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_combine_the_uri_with_an_endpoint()
    {
        var uri = new Uri("https://www.google.se/");

        const string expected = "https://www.google.se/foo/bar";
        var actual = uri.Combine("foo/bar").ToString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_change_the_scheme_to_http()
    {
        var uri = new Uri("https://www.google.se/");

        const string expected = "http://www.google.se/";
        var actual = uri.AsHttp().ToString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_change_the_scheme_to_https()
    {
        var uri = new Uri("http://www.google.se/");

        const string expected = "https://www.google.se/";
        var actual = uri.AsHttps().ToString();

        Assert.Equal(expected, actual);
    }
}
