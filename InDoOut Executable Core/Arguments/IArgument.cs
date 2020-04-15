namespace InDoOut_Executable_Core.Arguments
{
    public interface IArgument
    {
        bool Hidden { get; }
        bool AllowsValue { get; }
        string Key { get; }
        string Value { get; set; }
        string Description { get; }

        void Trigger(IArgumentHandler handler);
    }
}
