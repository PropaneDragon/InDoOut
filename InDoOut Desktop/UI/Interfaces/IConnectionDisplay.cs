using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IConnectionDisplay
    {
        void Remove(IUIConnection output);

        IUIConnection Create(IUIOutput start, Point end);
        IUIConnection Create(IUIOutput start, IUIInput end);
        IUIConnection FindConnection(IUIOutput output, IUIInput input);

        List<IUIConnection> FindConnections(IUIOutput output);
        List<IUIConnection> FindConnections(IUIInput input);
    }
}
