﻿using InDoOut_Desktop.Actions.Selecting;
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
        ISelectionManager SelectionManager { get; }
        BlockViewMode CurrentViewMode { get; set; }

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
