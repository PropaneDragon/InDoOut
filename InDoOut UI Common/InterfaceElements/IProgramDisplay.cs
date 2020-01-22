using InDoOut_Core.Entities.Programs;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IProgramDisplay : IFunctionDisplay, IConnectionDisplay
    {
        IProgram AssociatedProgram { get; set; }
    }
}
