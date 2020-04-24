using InDoOut_Core.Options;

namespace InDoOut_Executable_Core.Options
{
    public interface IAbstractProgramOptions
    {
        IOptionHolder OptionHolder { get; }

        void RegisterOptions();
    }
}