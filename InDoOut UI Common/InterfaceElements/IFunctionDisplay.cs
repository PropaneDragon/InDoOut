using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.Creation;
using System.Collections.Generic;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IFunctionDisplay
    {
        IFunctionCreator FunctionCreator { get; }

        List<IUIFunction> UIFunctions { get; }

        IUIFunction FindFunction(IFunction function);
    }
}
