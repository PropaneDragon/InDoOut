using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.Actions.Copying;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIProperty : IUIConnectionEnd, ICopyable
    {
        IProperty AssociatedProperty { get; set; }
    }
}
