using InDoOut_Core.Entities.Functions;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIProperty : IUIConnectionEnd
    {
        IProperty AssociatedProperty { get; set; }
    }
}
