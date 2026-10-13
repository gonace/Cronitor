using Cronitor.Models;
using Xunit;

namespace Cronitor.Tests;

public class MonitorTests
{
    [Fact]
    public void Should_set_a_reference_type_property_with_With()
    {
        var monitor = new Monitor("nightly-backup");

        monitor.With(x => x.Note, "note");

        Assert.Equal("note", monitor.Note);
    }

    [Fact]
    public void Should_set_an_int_property_with_With()
    {
        var monitor = new Monitor("nightly-backup");

        monitor.With(x => x.GraceSeconds, 300);

        Assert.Equal(300, monitor.GraceSeconds);
    }

    [Fact]
    public void Should_set_a_nullable_int_property_with_With()
    {
        var monitor = new Monitor("nightly-backup");

        monitor.With(x => x.FailureTolerance, 3);

        Assert.Equal(3, monitor.FailureTolerance);
    }

    [Fact]
    public void Should_set_a_nullable_bool_property_with_With()
    {
        var monitor = new Monitor("nightly-backup");

        monitor.With(x => x.Paused, true);

        Assert.True(monitor.Paused);
    }
}
