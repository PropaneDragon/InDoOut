using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIConnectionEnd : IUIConnectionPoint
    {
        void PositionUpdated(Point point);
    }
}
