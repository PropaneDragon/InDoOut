using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IFunctionDisplay
    {
        List<IUIFunction> UIFunctions { get; }

        IUIFunction Create(IFunction function);

        IUIFunction FindFunction(IFunction function);
    }
}
