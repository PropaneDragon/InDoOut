using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IConnectionCreator
    {
        IUIConnection Create(ICommonProgramDisplay display, IUIConnectionStart start, Point end);

        IUIConnection Create(ICommonProgramDisplay display, IUIConnectionStart start, IUIConnectionEnd end);
    }
}
