using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IFunctionDisplay
    {
        List<IUIFunction<IBlockView>> UIFunctions { get; }

        IUIFunction<IBlockView> Create(IFunction function);
        IUIFunction<IBlockView> Create(IFunction function, Point location);

        IUIFunction<IBlockView> FindFunction(IFunction function);
    }
}
