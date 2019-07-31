using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IElementDisplay
    {
        void Add(FrameworkElement element);
        void Add(FrameworkElement element, Point position);
        void Remove(FrameworkElement element);

        Point GetMousePosition();
        Point GetPosition(FrameworkElement element);
        Point GetBestSide(FrameworkElement element, Point point);
    }
}
