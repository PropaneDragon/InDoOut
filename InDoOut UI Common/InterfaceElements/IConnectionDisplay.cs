using InDoOut_UI_Common.Creation;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IConnectionDisplay
    {
        IConnectionCreator ConnectionCreator { get; }

        List<IUIConnection> UIConnections { get; }

        Point GetBestSide(Rect rectangle, Point point);
        Point GetBestSide(FrameworkElement element, Point point);
        Point GetBestSide(FrameworkElement element, FrameworkElement otherElement);

        IUIConnection FindConnection(IUIConnectionStart start, IUIConnectionEnd end);

        List<IUIConnection> FindConnections(IUIConnectionStart start);
        List<IUIConnection> FindConnections(IUIConnectionEnd end);
        List<IUIConnection> FindConnections(List<IUIConnectionStart> starts);
        List<IUIConnection> FindConnections(List<IUIConnectionEnd> ends);
    }
}
