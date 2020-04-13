using InDoOut_Executable_Core.Programs;

namespace InDoOut_UI_Common.InterfaceElements
{
    public enum ProgramViewMode
    {
        IO,
        Variables
    }

    public interface ICommonProgramDisplay : ICommonDisplay, IProgramDisplay, IFunctionDisplay, IConnectionDisplay
    {
        ProgramViewMode CurrentViewMode { get; set; }
    }
}
