using System;

namespace InDoOut_Executable_Core.Arguments
{
    public class BasicArgument : Argument
    {
        protected Action<IArgumentHandler> TriggerAction { get; set; }

        private BasicArgument(string key, string description = "", bool allowsValue = false, bool hidden = false) : base(key, description, allowsValue, hidden)
        {
        }

        private BasicArgument(string key, string description = "", string defaultValue = "", bool hidden = false) : base(key, description, defaultValue, hidden)
        {
        }

        public BasicArgument(string key, string description = "", bool allowsValue = false, bool hidden = false, Action<IArgumentHandler> triggerAction = null) : this(key, description, allowsValue, hidden)
        {
            TriggerAction = triggerAction;
        }

        public BasicArgument(string key, string description = "", string defaultValue = "", bool hidden = false, Action<IArgumentHandler> triggerAction = null) : this(key, description, defaultValue, hidden)
        {
            TriggerAction = triggerAction;
        }

        public override void Trigger(IArgumentHandler handler)
        {
            TriggerAction?.Invoke(handler);
        }
    }
}
