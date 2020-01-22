using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IFunctionDisplay
    {
        List<IUIFunction> UIFunctions { get; }

        IUIFunction Create(IFunction function);
        IUIFunction Create(IFunction function, Point location);

        IUIFunction FindFunction(IFunction function);
    }
}
