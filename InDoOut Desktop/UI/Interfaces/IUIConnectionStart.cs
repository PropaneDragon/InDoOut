using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIConnectionStart : IUIConnectionPoint
    {
        void PositionUpdated(Point position);
    }
}
