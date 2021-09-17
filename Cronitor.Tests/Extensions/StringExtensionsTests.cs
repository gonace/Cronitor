using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ShouldAddQueryString()
        {
            const string expected = "https://www.google.se?host=127.0.0.1&message=Lorem ipsum&env=Production&series=3de5db91-9c02-4e95-b8a9-9a2442702336&metric=count%3A9.99";

            const string address = "https://www.google.se";
            const string queryString = "?host=127.0.0.1&message=Lorem ipsum&env=Production&series=3de5db91-9c02-4e95-b8a9-9a2442702336&metric=count%3A9.99";
            var actual = address.AddQueryString(queryString);


            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldEncodeBase64()
        {
            const string expected = "ODc2NmMzMGMzNzU1NDhlNzk1NWRhNTIxYzBiM2ZkMGU6";
            var actual = "8766c30c375548e7955da521c0b3fd0e:".Base64Encode();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldDecodeBase64()
        {
            const string expected = "8766c30c375548e7955da521c0b3fd0e:";
            var actual = "ODc2NmMzMGMzNzU1NDhlNzk1NWRhNTIxYzBiM2ZkMGU6".Base64Decode();

            Assert.Equal(expected, actual);
        }
    }
}
