using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display.UI.Controls.Screens
{
    public enum ScreenEdge
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

    public enum ScreenMode
    {
        Layout,
        Connections
    }

    public interface IScreen : IElementDisplay, IDisplayElementDisplay
    {
        ScreenMode Mode { get; set; }
        ISelectionManager<ISelectable> SelectionManager { get; }
        Size Size { get; }

        ScreenEdge GetCloseEdge(Point point, double distance = 5);
        bool PointCloseToScreenItemEdge(Point point, double distance = 5);
    }
}