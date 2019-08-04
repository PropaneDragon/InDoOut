using InDoOut_Core.Entities.Core;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public enum BlockViewMode
    {
        IO,
        Variables
    }

    public interface IBlockView : IProgramDisplay, IConnectionDisplay, IElementDisplay
    {
        BlockViewMode CurrentViewMode { get; set; }

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
