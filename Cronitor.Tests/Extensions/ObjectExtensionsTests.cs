using System;
using Cronitor.Attributes;
using Cronitor.Extensions;
using Xunit;

namespace Cronitor.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void ShouldConvertObjectToQueryString()
        {
            const string expected = "?name=Jane Doe&age=18&wage=25400,99&working=true&startedat=2019-06-01 08:00:00";
            var actual = new Model().ToQueryString();

            Assert.Equal(expected, actual);
        }
    }

    public class Model
    {
        [QueryString]
        public string Name => "Jane Doe";
        [QueryString]
        public int Age => 18;
        [QueryString("wage")]
        public decimal Salary => new decimal(25400.99);
        [QueryString("working", true)]
        public bool IsWorking => true;
        [QueryString]
        public DateTime StartedAt => DateTime.Parse("2019-06-01T08:00:00");
    }
}
