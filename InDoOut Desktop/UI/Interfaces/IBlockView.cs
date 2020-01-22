using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public enum BlockViewMode
    {
        IO,
        Variables
    }

    public interface IBlockView : IProgramDisplay, IElementDisplay, IFunctionDisplay, IConnectionDisplay
    {
        ISelectionManager<ISelectable> SelectionManager { get; }
        BlockViewMode CurrentViewMode { get; set; }

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
