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

        /// <summary>
        /// Convert property to lower case.
        /// </summary>
        public bool Lower { get; set; } = false;

        public QueryStringAttribute()
        {
        }

        public QueryStringAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public QueryStringAttribute(string propertyName, bool lower)
        {
            PropertyName = propertyName;
            Lower = lower;
        }

        public QueryStringAttribute(bool lower)
        {
            Lower = lower;
        }
    }
}
