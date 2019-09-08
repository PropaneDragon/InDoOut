using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.Display.Selection;
using System.Collections.Generic;

namespace InDoOut_Desktop.UI.Interfaces
{
    public enum UIFunctionDisplayMode
    {
        None,
        IO,
        Variables
    }

    public interface IUIFunction : IDraggable, ISelectable
    {
        UIFunctionDisplayMode DisplayMode { get; set; }
        IFunction AssociatedFunction { get; set; }
        List<IUIInput> Inputs { get; }
        List<IUIOutput> Outputs { get; }
        List<IUIProperty> Properties { get; }
        List<IUIResult> Results { get; }
    }
}
