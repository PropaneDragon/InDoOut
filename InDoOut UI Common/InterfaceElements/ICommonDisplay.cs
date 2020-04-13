using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.Removal;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface ICommonDisplay : IElementDisplay
    {
        IDeletableRemover DeletableRemover { get; }
        IActionHandler ActionHandler { get; }
        ISelectionManager<ISelectable> SelectionManager { get; }

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
