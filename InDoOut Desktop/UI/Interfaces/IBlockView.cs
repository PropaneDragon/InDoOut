using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public enum BlockViewMode
    {
        IO,
        Variables
    }

    public interface IBlockView : IProgramDisplay, IElementDisplay
    {
        BlockViewMode CurrentViewMode { get; set; }

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
