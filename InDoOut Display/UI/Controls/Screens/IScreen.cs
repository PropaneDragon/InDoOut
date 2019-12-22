using InDoOut_Display.Actions.Selecting;
using InDoOut_UI_Common.Actions.Selecting;
using System.Windows;

namespace InDoOut_Display.UI.Controls.Screens
{
    public enum ScreenItemEdge
    {
        None,
        Left,
        Top,
        Right,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public interface IScreen : IElementHost, IDisplayElementHost
    {
        ISelectionManager<IScreenSelectable> SelectionManager { get; }
        Size Size { get; }

        ScreenItemEdge GetCloseEdge(Point point, double distance = 5);
        bool PointCloseToScreenItemEdge(Point point, double distance = 5);
        Point GetMousePosition();
    }
}