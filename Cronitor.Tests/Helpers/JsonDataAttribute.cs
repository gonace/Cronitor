using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace Cronitor.Tests.Helpers
{
    public class JsonDataAttribute : DataAttribute
    {
        private readonly string _filePath;

        public JsonDataAttribute(string filePath)
        {
            _filePath = filePath;
        }

        public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
        {
            var directoryPath = AppContext.BaseDirectory;

            var file = string.Concat(directoryPath, "\\Files\\", _filePath.Replace("/", "\\"));
            var content = File.ReadAllText(file, Encoding.UTF8);
            var data = new List<ITheoryDataRow> { new TheoryDataRow(content) };

            return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>(data);
        }

        public override bool SupportsDiscoveryEnumeration()
        {
            return true;
        }
    }
}