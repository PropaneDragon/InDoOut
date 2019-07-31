using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IBlockView : IProgramDisplay, IConnectionDisplay, IElementDisplay
    {
        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
