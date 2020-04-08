using InDoOut_Core.Entities.Programs;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Executable_Core.Programs
{
    public interface IProgramDisplay : IFunctionDisplay, IConnectionDisplay
    {
        IProgram AssociatedProgram { get; set; }
    }
}
