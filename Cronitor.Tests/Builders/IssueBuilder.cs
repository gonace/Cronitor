using System;
using Cronitor.Models;

namespace Cronitor.Tests.Builders
{
    public class IssueBuilder
    {
        private string _key = "Key";
        private string _name = "Name";
        private DateTime _createdAt = DateTime.UtcNow.AddMinutes(-5);
        private DateTime _updatedAt = DateTime.UtcNow;

        public Issue Build()
        {
            return new Issue
            {
                Key = _key,
                Name = _name,
                CreatedAt = _createdAt,
                UpdatedAt = _updatedAt
            };
        }

        public IssueBuilder Key(string key)
        {
            _key = key;
            return this;
        }
    }
}