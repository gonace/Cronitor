using System;

namespace Cronitor.Attributes
{
    /// <summary>
    /// Marks a property that Cronitor sets, such as a monitor's passing or created:
    /// it is read from responses but never sent in requests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class ResponseOnlyAttribute : Attribute
    {
    }
}
