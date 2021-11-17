using System;

namespace InDoOut_Networking.Shared.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class ExtractPropertyAttribute : Attribute
    {
        public bool IgnoreMissingProperty { get; set; } = false;

        public string FromPropertyName { get; set; } = null;
        public string ToPropertyName { get; set; } = null;

        public ExtractPropertyAttribute(string fromProperty, string toProperty, bool ignoreMissing = false)
        {
            FromPropertyName = fromProperty ?? toProperty;
            ToPropertyName = toProperty ?? fromProperty;
            IgnoreMissingProperty = ignoreMissing;
        }

        public ExtractPropertyAttribute(string property, bool ignoreMissing = false) : this(property, property, ignoreMissing)
        {
        }
    }
}
