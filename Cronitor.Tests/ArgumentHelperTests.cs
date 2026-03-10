using System;
using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests
{
    public class ArgumentHelperTests
    {
        [Fact]
        public void ThrowIfNullOrWhiteSpace_WithNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace(null));
        }

        [Fact]
        public void ThrowIfNullOrWhiteSpace_WithEmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace(""));
        }

        [Fact]
        public void ThrowIfNullOrWhiteSpace_WithWhitespace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("   "));
        }

        [Fact]
        public void ThrowIfNullOrWhiteSpace_WithValidString_DoesNotThrow()
        {
            var exception = Record.Exception(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("valid"));

            Assert.Null(exception);
        }

        [Fact]
        public void ThrowIfNullOrWhiteSpace_WithTab_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("\t"));
        }

        [Fact]
        public void ThrowIfNullOrWhiteSpace_WithNewline_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentHelper.ThrowIfNullOrWhiteSpace("\n"));
        }
    }
}