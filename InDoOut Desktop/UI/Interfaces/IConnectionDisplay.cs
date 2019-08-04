using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IConnectionDisplay
    {
        void Remove(IUIConnection output);

        IUIConnection Create(IUIConnectionStart start, Point end);
        IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end);
        IUIConnection FindConnection(IUIConnectionStart start, IUIConnectionEnd end);

        List<IUIConnection> FindConnections(IUIConnectionStart start);
        List<IUIConnection> FindConnections(IUIConnectionEnd end);
        List<IUIConnection> FindConnections(List<IUIConnectionStart> starts);
        List<IUIConnection> FindConnections(List<IUIConnectionEnd> ends);
    }
}
