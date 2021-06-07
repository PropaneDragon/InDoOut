using System;

namespace InDoOut_Networking.Shared.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class ExtractPropertyAttribute : Attribute
    {
        public string FromPropertyName { get; set; } = null;
        public string ToPropertyName { get; set; } = null;

        public ExtractPropertyAttribute(string fromProperty = null, string toProperty = null)
        {
            FromPropertyName = fromProperty ?? toProperty;
            ToPropertyName = toProperty ?? fromProperty;
        }
    }
}
