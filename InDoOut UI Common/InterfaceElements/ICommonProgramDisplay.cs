using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public enum ProgramViewMode
    {
        IO,
        Variables
    }

    public interface ICommonProgramDisplay : IProgramDisplay, IElementDisplay, IFunctionDisplay, IConnectionDisplay
    {
        ProgramViewMode CurrentViewMode { get; set; }
        ISelectionManager<ISelectable> SelectionManager { get; }
        IActionHandler ActionHandler { get; }

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
