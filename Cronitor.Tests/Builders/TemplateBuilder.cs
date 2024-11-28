using System;
using System.Collections.Generic;
using Cronitor.Models;

namespace Cronitor.Tests.Builders
{
    public class TemplateBuilder
    {
        private string _key = "Key";
        private string _name = "Name";
        private DateTime _createrdAt = DateTime.UtcNow;
        private string _status = "0 recipients used by 5 monitors";

        private Notifications _notifications = new Notifications
        {
            Emails = new List<string> { "jane.doe@domain.tld", "john.doe@domain.tld" }
        };

        private List<string> _monitors = new List<string>() { "Monitor1", "Monitor2", "Monitor3" };

        public Template Build()
        {
            return new Template
            {
                Key = _key,
                Name = _name,
                Monitors = _monitors,
                CreatedAt = _createrdAt,
                Notifications = _notifications,
                Status = _status
            };
        }
    }
}