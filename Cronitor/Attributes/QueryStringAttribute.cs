using System;

namespace Cronitor.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryStringAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }

        public QueryStringAttribute()
        {
        }

        public QueryStringAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
