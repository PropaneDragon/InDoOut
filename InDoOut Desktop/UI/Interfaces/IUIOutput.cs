using InDoOut_Core.Entities.Functions;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIOutput : IUIConnectionStart
    {
        IOutput AssociatedOutput { get; set; }
    }
}
