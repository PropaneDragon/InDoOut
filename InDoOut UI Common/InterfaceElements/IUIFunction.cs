using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.Actions.Copying;
using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using System.Collections.Generic;

namespace InDoOut_UI_Common.InterfaceElements
{
    public enum UIFunctionDisplayMode
    {
        None,
        IO,
        Variables
    }

    public interface IUIFunction : IDraggable, ICopyable, IDeletable, ISelectable
    {
        UIFunctionDisplayMode DisplayMode { get; set; }
        IFunction AssociatedFunction { get; set; }
        List<IUIInput> Inputs { get; }
        List<IUIOutput> Outputs { get; }
        List<IUIProperty> Properties { get; }
        List<IUIResult> Results { get; }
    }
}
