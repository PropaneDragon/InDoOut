using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Display.UI.Controls.Screens
{
    public interface IElementHost
    {
        FrameworkElement GetElementUnderMouse();
        FrameworkElement GetElementAtPoint(Point point);

        List<FrameworkElement> GetElementsUnderMouse();
        List<FrameworkElement> GetElementsAtPoint(Point point);

        T GetFirstElementOfType<T>(FrameworkElement element) where T : class;
        T GetFirstElementOfType<T>(List<FrameworkElement> elements) where T : class;
    }
}
