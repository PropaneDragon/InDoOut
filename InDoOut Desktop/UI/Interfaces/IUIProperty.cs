using InDoOut_Core.Entities.Functions;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIProperty : IUIConnectionEnd
    {
        IProperty AssociatedProperty { get; set; }
    }
}
