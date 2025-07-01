using System;
using System.Collections.Generic;
using Cronitor.Models;

namespace Cronitor.Tests.Builders
{
    public class TemplateBuilder
    {
        private readonly string _key = "Key";
        private readonly string _name = "Name";
        private readonly DateTime _createddAt = DateTime.UtcNow;
        private readonly string _status = "0 recipients used by 5 monitors";

        private readonly Notifications _notifications = new Notifications
        {
            Emails = new List<string> { "jane.doe@domain.tld", "john.doe@domain.tld" }
        };

        private readonly List<string> _monitors = new List<string>() { "Monitor1", "Monitor2", "Monitor3" };

        public Template Build()
        {
            return new Template
            {
                Key = _key,
                Name = _name,
                Monitors = _monitors,
                CreatedAt = _createddAt,
                Notifications = _notifications,
                Status = _status
            };
        }
    }
}