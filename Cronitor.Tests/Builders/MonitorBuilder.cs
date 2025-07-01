
using Cronitor.Models;

namespace Cronitor.Tests.Builders
{
    public class MonitorBuilder
    {
        private readonly string _key = "Key";
        private readonly string _name = "Name";

        public Monitor Build()
        {
            return new Monitor(_key)
            {
                Name = _name
            };
        }
    }
}