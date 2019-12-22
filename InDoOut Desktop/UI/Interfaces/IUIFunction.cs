using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions.Copying;
using InDoOut_Desktop.Actions.Deleting;
using InDoOut_Desktop.Actions.Dragging;
using InDoOut_Desktop.Actions.Selecting;
using System.Collections.Generic;

namespace InDoOut_Desktop.UI.Interfaces
{
    public enum UIFunctionDisplayMode
    {
        None,
        IO,
        Variables
    }

    public interface IUIFunction : IDraggable, ICopyable, IDeletable, IBlockViewSelectable
    {
        UIFunctionDisplayMode DisplayMode { get; set; }
        IFunction AssociatedFunction { get; set; }
        List<IUIInput> Inputs { get; }
        List<IUIOutput> Outputs { get; }
        List<IUIProperty> Properties { get; }
        List<IUIResult> Results { get; }
    }
}
