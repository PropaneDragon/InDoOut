using InDoOut_Core.Entities.Functions;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIOutput : IUIConnectionStart
    {
        IOutput AssociatedOutput { get; set; }
    }
}
