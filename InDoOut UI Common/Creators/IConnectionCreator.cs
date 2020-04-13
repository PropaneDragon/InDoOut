using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Creators
{
    public interface IConnectionCreator
    {
        IUIConnection Create(IUIConnectionStart start, Point end);

        IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end);
    }
}
