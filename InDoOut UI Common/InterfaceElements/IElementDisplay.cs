using System.Collections.Generic;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IElementDisplay
    {
        List<FrameworkElement> Elements { get; }

        void Add(FrameworkElement element);
        void Add(FrameworkElement element, Point position, int zIndex = 0);
        void Remove(FrameworkElement element);
        void SetPosition(FrameworkElement element, Point position);

        Point GetMousePosition();
        Point GetPosition(FrameworkElement element);

        Size GetSize(FrameworkElement element);

        List<FrameworkElement> GetElementsUnderMouse();
        List<FrameworkElement> GetElementsAtPoint(Point point);

        T GetFirstElementOfType<T>(FrameworkElement element) where T : class;
        T GetFirstElementOfType<T>(List<FrameworkElement> elements) where T : class;
    }
}
