using InDoOut_Executable_Core.Arguments;

namespace InDoOut_Console.Arguments
{
    public class ConsoleProgramRunArgument : Argument
    {
        public ConsoleProgramRunArgument(int id, string defaultValue = "") : base($"{id}", $"An argument to pass into the active program at ID {id} when it begins running", defaultValue, false)
        {
        }

        public override void Trigger(IArgumentHandler handler)
        {

        }
    }
}
