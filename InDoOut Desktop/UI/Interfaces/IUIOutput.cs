using InDoOut_Core.Entities.Functions;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIOutput
    {
        IOutput AssociatedOutput { get; set; }
    }
}
