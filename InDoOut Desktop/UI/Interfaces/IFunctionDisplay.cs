using InDoOut_Core.Entities.Functions;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IFunctionDisplay
    {
        IUIFunction Create(IFunction function);
        IUIFunction Create(IFunction function, Point location);

        IUIFunction FindFunction(IFunction function);
    }
}
