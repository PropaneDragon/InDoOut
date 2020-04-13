using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Creation
{
    public interface IConnectionCreator : IElementCreator
    {
        IUIConnection Create(IUIConnectionStart start, Point end);

        IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end);
    }
}
