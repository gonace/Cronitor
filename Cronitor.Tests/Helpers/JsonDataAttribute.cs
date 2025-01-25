using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Cronitor.Tests.Helpers
{
    public class JsonDataAttribute : DataAttribute
    {
        private readonly string _filePath;

        public JsonDataAttribute(string filePath)
        {
            _filePath = filePath;
        }

        public override IEnumerable<object[]> GetData(MethodInfo method)
        {
            var assemblyPath = typeof(JsonDataAttribute).Assembly.Location;
            assemblyPath = assemblyPath.Remove(assemblyPath.LastIndexOf("\\", StringComparison.Ordinal));

            var file = string.Concat(assemblyPath, "\\Files\\", _filePath.Replace("/", "\\"));
            var content = File.ReadAllText(file, Encoding.UTF8);

            return new List<object[]> { new object[] { content } };
        }
    }
}