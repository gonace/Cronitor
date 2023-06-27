using Cronitor.Attributes;
using Cronitor.Extensions;
using Cronitor.Tests.Helpers;
using System;
using Xunit;

namespace Cronitor.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        [UseCulture("en-US")]
        public void ShouldConvertObjectToQueryStringForCultureEnUs()
        {
            const string expected = "?name=Jane Doe&age=18&wage=25400.99&working=true&starts_at=6/1/2019 8:00:00 AM";
            var actual = new Model().ToQueryString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        [UseCulture("sv-SE")]
        public void ShouldConvertObjectToQueryStringSvSe()
        {
            const string expected = "?name=Jane Doe&age=18&wage=25400,99&working=true&starts_at=2019-06-01 08:00:00";
            var actual = new Model().ToQueryString();

            Assert.Equal(expected, actual);
        }

        private class Model
        {
            [QueryStringProperty]
            public string Name => "Jane Doe";
            [QueryStringProperty]
            public int Age => 18;
            [QueryStringProperty("wage")]
            public decimal Salary => new decimal(25400.99);
            [QueryStringProperty("working", true)]
            public bool IsWorking => true;
            [QueryStringProperty("starts_at")]
            public DateTime StartedAt => DateTime.Parse("2019-06-01T08:00:00");
        }
    }
}
