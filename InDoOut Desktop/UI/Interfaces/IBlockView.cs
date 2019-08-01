using InDoOut_Core.Entities.Core;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IBlockView : IProgramDisplay, IConnectionDisplay, IElementDisplay
    {
        void AssociateEntityWithUI(IEntity entity, FrameworkElement element);

        Size TotalSize { get; }
        Size ViewSize { get; }

        Point TopLeftViewCoordinate { get; }
        Point BottomRightViewCoordinate { get; }
        Point CentreViewCoordinate { get; }
    }
}
