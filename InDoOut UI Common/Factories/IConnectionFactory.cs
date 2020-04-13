using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Factories
{
    public interface IConnectionFactory
    {
        IUIConnection Create(ICommonProgramDisplay display, IUIConnectionStart start, Point end);

        IUIConnection Create(ICommonProgramDisplay display, IUIConnectionStart start, IUIConnectionEnd end);
    }
}
