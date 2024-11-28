
using Cronitor.Models;

namespace Cronitor.Tests.Builders
{
    public class MonitorBuilder
    {
        private string _key = "Key";
        private string _name = "Name";

        public Monitor Build()
        {
            return new Monitor(_key)
            {
                Name = _name
            };
        }
    }
}