using System;

namespace Cronitor.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryStringPropertyAttribute : Attribute
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

        public QueryStringPropertyAttribute()
        {
        }

        public QueryStringPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public QueryStringPropertyAttribute(string propertyName, bool lower)
        {
            PropertyName = propertyName;
            Lower = lower;
        }

        public QueryStringPropertyAttribute(bool lower)
        {
            Lower = lower;
        }
    }
}
