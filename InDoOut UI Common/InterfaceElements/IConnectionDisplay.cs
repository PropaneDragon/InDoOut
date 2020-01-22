using System.Collections.Generic;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IConnectionDisplay
    {
        List<IUIConnection> UIConnections { get; }

        void Remove(IUIConnection connection);

        Point GetBestSide(Rect rectangle, Point point);
        Point GetBestSide(FrameworkElement element, Point point);
        Point GetBestSide(FrameworkElement element, FrameworkElement otherElement);

        IUIConnection Create(IUIConnectionStart start, Point end);
        IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end);
        IUIConnection FindConnection(IUIConnectionStart start, IUIConnectionEnd end);

        List<IUIConnection> FindConnections(IUIConnectionStart start);
        List<IUIConnection> FindConnections(IUIConnectionEnd end);
        List<IUIConnection> FindConnections(List<IUIConnectionStart> starts);
        List<IUIConnection> FindConnections(List<IUIConnectionEnd> ends);
    }
}
