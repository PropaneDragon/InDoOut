using InDoOut_UI_Common.InterfaceElements;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IConnectionDisplay
    {
        List<IUIConnection<IBlockView>> UIConnections { get; }

        void Remove(IUIConnection<IBlockView> connection);

        IUIConnection<IBlockView> Create(IUIConnectionStart start, Point end);
        IUIConnection<IBlockView> Create(IUIConnectionStart start, IUIConnectionEnd end);
        IUIConnection<IBlockView> FindConnection(IUIConnectionStart start, IUIConnectionEnd end);

        List<IUIConnection<IBlockView>> FindConnections(IUIConnectionStart start);
        List<IUIConnection<IBlockView>> FindConnections(IUIConnectionEnd end);
        List<IUIConnection<IBlockView>> FindConnections(List<IUIConnectionStart> starts);
        List<IUIConnection<IBlockView>> FindConnections(List<IUIConnectionEnd> ends);
    }
}
