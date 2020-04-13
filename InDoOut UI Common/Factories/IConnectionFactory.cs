using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Factories
{
    public interface IConnectionFactory
    {
        IUIConnection Create(IUIConnectionStart start, Point end);

        IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end);
    }
}
