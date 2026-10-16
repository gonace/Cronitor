using System;
using Xunit;

namespace Cronitor.Tests;

public class MonitorKeyTests
{
    private const string Smiley = "\U0001F600";

    [Fact]
    public void Should_keep_a_valid_key()
    {
        var key = new MonitorKey("Nightly-Backup");

        Assert.Equal("Nightly-Backup", key.ToString());
    }

    [Fact]
    public void Should_accept_whitespace_inside_a_key()
    {
        var key = new MonitorKey("nightly backup");

        Assert.Equal("nightly backup", key.ToString());
    }

    [Fact]
    public void Should_accept_characters_that_need_escaping_in_a_url()
    {
        var key = new MonitorKey("a?b#c%d");

        Assert.Equal("a?b#c%d", key.ToString());
    }

    [Fact]
    public void Should_accept_a_key_of_100_code_points()
    {
        var value = new string('x', 100);

        var key = new MonitorKey(value);

        Assert.Equal(value, key.ToString());
    }

    [Fact]
    public void Should_count_an_emoji_as_one_code_point()
    {
        var value = new string('x', 99) + Smiley;

        var key = new MonitorKey(value);

        Assert.Equal(value, key.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Should_reject_a_blank_key(string value)
    {
        Assert.Throws<ArgumentNullException>(() => new MonitorKey(value));
    }

    [Fact]
    public void Should_reject_a_key_containing_a_slash()
    {
        var exception = Assert.Throws<ArgumentException>(() => new MonitorKey("reports/daily"));

        Assert.Contains("'/'", exception.Message);
    }

    [Fact]
    public void Should_reject_a_key_longer_than_100_code_points()
    {
        var exception = Assert.Throws<ArgumentException>(() => new MonitorKey(new string('x', 100) + Smiley));

        Assert.Contains("100", exception.Message);
    }

    [Theory]
    [InlineData(" nightly-backup")]
    [InlineData("nightly-backup ")]
    [InlineData("\tnightly-backup")]
    public void Should_reject_a_key_with_leading_or_trailing_whitespace(string value)
    {
        Assert.Throws<ArgumentException>(() => new MonitorKey(value));
    }

    [Theory]
    [InlineData("nightly", '\uDC00', "")]
    [InlineData("nightly", '\uD800', "")]
    [InlineData("nightly", '\uD800', "-backup")]
    public void Should_reject_a_key_with_an_incomplete_character(string before, char half, string after)
    {
        var exception = Assert.Throws<ArgumentException>(() => new MonitorKey(before + half + after));

        Assert.Contains("Unicode", exception.Message);
    }

    [Fact]
    public void Should_equal_a_key_that_differs_only_in_letter_case()
    {
        var lower = new MonitorKey("nightly-backup");
        var upper = new MonitorKey("Nightly-Backup");

        Assert.True(lower.Equals(upper));
        Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
    }

    [Fact]
    public void Should_not_equal_a_different_key()
    {
        var nightly = new MonitorKey("nightly-backup");
        var weekly = new MonitorKey("weekly-backup");

        Assert.False(nightly.Equals(weekly));
    }

    [Fact]
    public void Should_convert_implicitly_from_a_valid_string()
    {
        MonitorKey key = "nightly-backup";

        Assert.Equal("nightly-backup", key.ToString());
    }

    [Fact]
    public void Should_throw_when_an_invalid_string_is_converted()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            MonitorKey key = "reports/daily";
            return key;
        });
    }

    [Fact]
    public void Should_convert_implicitly_to_a_string()
    {
        var key = new MonitorKey("nightly-backup");

        string value = key;

        Assert.Equal("nightly-backup", value);
    }

    [Fact]
    public void Should_create_a_key_that_follows_the_rules()
    {
        var created = MonitorKey.TryCreate("Nightly-Backup", out var key);

        Assert.True(created);
        Assert.Equal("Nightly-Backup", key.ToString());
    }

    [Fact]
    public void Should_not_create_a_key_containing_a_slash_instead_of_changing_it()
    {
        var created = MonitorKey.TryCreate("reports/daily", out var key);

        Assert.False(created);
        Assert.Null(key);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(" nightly-backup")]
    public void Should_not_create_an_invalid_key(string value)
    {
        var created = MonitorKey.TryCreate(value, out var key);

        Assert.False(created);
        Assert.Null(key);
    }

    [Fact]
    public void Should_not_create_a_key_longer_than_100_characters()
    {
        var created = MonitorKey.TryCreate(new string('x', 101), out var key);

        Assert.False(created);
        Assert.Null(key);
    }

    [Fact]
    public void Should_replace_slashes_in_a_name()
    {
        var derived = MonitorKey.TryFromName("reports/daily", out var key);

        Assert.True(derived);
        Assert.Equal("reports-daily", key.ToString());
    }

    [Fact]
    public void Should_trim_a_name()
    {
        var derived = MonitorKey.TryFromName(" nightly-backup ", out var key);

        Assert.True(derived);
        Assert.Equal("nightly-backup", key.ToString());
    }

    [Fact]
    public void Should_not_derive_a_key_from_a_name_longer_than_100_code_points()
    {
        var derived = MonitorKey.TryFromName(new string('x', 101), out var key);

        Assert.False(derived);
        Assert.Null(key);
    }

    [Fact]
    public void Should_not_derive_a_key_from_a_name_with_an_incomplete_character()
    {
        var derived = MonitorKey.TryFromName("nightly" + '\uDC00', out var key);

        Assert.False(derived);
        Assert.Null(key);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Should_not_derive_a_key_from_a_blank_name(string name)
    {
        var derived = MonitorKey.TryFromName(name, out var key);

        Assert.False(derived);
        Assert.Null(key);
    }
}
