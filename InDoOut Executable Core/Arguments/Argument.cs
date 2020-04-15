namespace InDoOut_Executable_Core.Arguments
{
    public abstract class Argument : IArgument
    {
        public bool Hidden { get; protected set; } = false;
        public bool AllowsValue { get; protected set; } = false;
        public string Key { get; protected set; }
        public string Value { get; set; }
        public string Description { get; protected set; } = "";

        public Argument(string key, string description = "", bool allowsValue = false, bool hidden = false)
        {
            Key = key;
            Description = description;
            AllowsValue = allowsValue;
            Hidden = hidden;
        }

        public Argument(string key, string description = "", string defaultValue = "", bool hidden = false) : this(key, description, true, hidden)
        {
            Value = defaultValue;
        }

        public abstract void Trigger(IArgumentHandler handler);

        public override string ToString()
        {
            return $"{Key} [{Value}]";
        }
    }
}
