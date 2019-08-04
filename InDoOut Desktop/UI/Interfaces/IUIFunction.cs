using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;

namespace InDoOut_Desktop.UI.Interfaces
{
    public enum UIFunctionDisplayMode
    {
        IO,
        Variables
    }

    public interface IUIFunction
    {
        UIFunctionDisplayMode DisplayMode { get; set; }
        IFunction AssociatedFunction { get; set; }
        List<IUIInput> Inputs { get; }
        List<IUIOutput> Outputs { get; }
        List<IUIProperty> Properties { get; }
        List<IUIResult> Results { get; }
    }
}
