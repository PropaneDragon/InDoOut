using System;

namespace InDoOut_Executable_Core.Networking
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandResponseAttribute : Attribute
    {
        public string AssociatedCommandName { get; private set; }

        private CommandResponseAttribute()
        {
        }

        public CommandResponseAttribute(string associatedCommandName) : this()
        {
            AssociatedCommandName = associatedCommandName;
        }
    }
}
