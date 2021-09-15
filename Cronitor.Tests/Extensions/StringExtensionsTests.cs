using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ChangesSchemeToHttp()
        {
            var encoded = "8766c30c375548e7955da521c0b3fd0e:".Base64Encode();

            Assert.Equal("ODc2NmMzMGMzNzU1NDhlNzk1NWRhNTIxYzBiM2ZkMGU6", encoded);
        }

        [Fact]
        public void ChangesSchemeToHttps()
        {
            var decoded = "ODc2NmMzMGMzNzU1NDhlNzk1NWRhNTIxYzBiM2ZkMGU6".Base64Decode();

            Assert.Equal("8766c30c375548e7955da521c0b3fd0e:", decoded);
        }
    }
}
