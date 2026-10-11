using System;
using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests;

public class ArgumentHelperTests
{
    [Fact]
    public void Should_throw_ArgumentException_when_ThrowIfNullOrWhiteSpace_gets_null()
    {
        Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace(null));
    }

    [Fact]
    public void Should_throw_ArgumentException_when_ThrowIfNullOrWhiteSpace_gets_an_empty_string()
    {
        Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace(""));
    }

    [Fact]
    public void Should_throw_ArgumentException_when_ThrowIfNullOrWhiteSpace_gets_whitespace()
    {
        Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("   "));
    }

    [Fact]
    public void Should_not_throw_when_ThrowIfNullOrWhiteSpace_gets_text()
    {
        var exception = Record.Exception(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("valid"));

        Assert.Null(exception);
    }

    [Fact]
    public void Should_throw_ArgumentException_when_ThrowIfNullOrWhiteSpace_gets_a_tab()
    {
        Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("\t"));
    }

    [Fact]
    public void Should_throw_ArgumentException_when_ThrowIfNullOrWhiteSpace_gets_a_newline()
    {
        Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("\n"));
    }
}
