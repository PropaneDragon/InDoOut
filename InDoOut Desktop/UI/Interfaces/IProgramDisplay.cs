using InDoOut_Core.Entities.Programs;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IProgramDisplay : IFunctionDisplay, IConnectionDisplay
    {
        IProgram AssociatedProgram { get; set; }
    }
}
