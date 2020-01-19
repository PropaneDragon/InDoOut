using InDoOut_Core.Entities.Functions;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIInput : IUIConnectionEnd
    {
        IInput AssociatedInput { get; set; }
    }
}
