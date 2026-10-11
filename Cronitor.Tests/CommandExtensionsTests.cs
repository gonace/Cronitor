using Cronitor.Attributes;
using Cronitor.Commands;
using Cronitor.Extensions;
using Cronitor.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Xunit;

namespace Cronitor.Tests
{
    public class CommandExtensionsTests
    {
        [Fact]
        [UseCulture("en-US")]
        public void ShouldConvertObjectToQueryStringForCultureEnUs()
        {
            const string expected = "?name=Jane%20Doe&age=18&wage=25400.99&isonsickleave=true&working=true&starts_at=6%2F1%2F2019%208%3A00%3A00%20AM";
            var uri = new Uri("https://www.cronitor.io");
            var actual = new Model(uri, HttpMethod.Get, "endpoint").ToQueryString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        [UseCulture("sv-SE")]
        public void ShouldConvertObjectToQueryStringSvSe()
        {
            const string expected = "?name=Jane%20Doe&age=18&wage=25400%2C99&isonsickleave=true&working=true&starts_at=2019-06-01%2008%3A00%3A00";
            var uri = new Uri("https://www.cronitor.io");
            var actual = new Model(uri, HttpMethod.Get, "endpoint").ToQueryString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldRepeatNameForEveryValueInACollection()
        {
            var uri = new Uri("https://www.cronitor.io");
            var actual = new CollectionModel(uri, new[] { "a", "b" }).ToQueryString();

            Assert.Equal("?tag=a&tag=b", actual);
        }

        [Fact]
        public void ShouldLeaveOutAnEmptyCollection()
        {
            var uri = new Uri("https://www.cronitor.io");
            var actual = new CollectionModel(uri, Array.Empty<string>()).ToQueryString();

            Assert.Equal(string.Empty, actual);
        }

        private class CollectionModel : Command
        {
            public CollectionModel(Uri uri, IReadOnlyList<string> tags)
                : base(uri, HttpMethod.Get, "endpoint")
            {
                Tags = tags;
            }

            [QueryStringProperty("tag")]
            public IReadOnlyList<string> Tags { get; }
        }

        private class Model : Command
        {
            protected Model(Uri uri)
                : base(uri)
            {
            }

            public Model(Uri uri, HttpMethod method, string endpoint)
                : base(uri, method, endpoint)
            {
            }

            [QueryStringProperty]
            public string Name => "Jane Doe";
            [QueryStringProperty]
            public int Age => 18;
            [QueryStringProperty("wage")]
            public decimal Salary => new decimal(25400.99);
            [QueryStringProperty(true)]
            public bool IsOnSickLeave => true;
            [QueryStringProperty("working", true)]
            public bool IsWorking => true;
            [QueryStringProperty("starts_at")]
            public DateTime StartedAt => DateTime.Parse("2019-06-01T08:00:00", new CultureInfo("en-US"));
        }
    }
}
