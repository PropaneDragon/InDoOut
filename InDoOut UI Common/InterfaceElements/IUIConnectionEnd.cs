using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIConnectionEnd : IUIConnectionPoint
    {
        void PositionUpdated(Point point);
    }
}
