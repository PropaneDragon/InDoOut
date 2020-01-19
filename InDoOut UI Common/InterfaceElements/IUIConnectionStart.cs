using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIConnectionStart : IUIConnectionPoint
    {
        void PositionUpdated(Point position);
    }
}
