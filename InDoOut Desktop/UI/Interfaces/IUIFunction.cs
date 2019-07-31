using InDoOut_Core.Entities.Functions;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIFunction
    {
        IFunction AssociatedFunction { get; set; }
    }
}
