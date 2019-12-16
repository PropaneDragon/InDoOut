using InDoOut_Display.UI.Controls.Screens;
using System.Windows;

namespace InDoOut_Display.Actions.Resizing
{
    public enum ResizeEdge
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

    public interface IResizable
    {
        void ResizeStarted(IScreen screen);
        void ResizeEnded(IScreen screen);

        bool CanResize(IScreen screen);
        bool CloseToEdge(IScreen screen, Point point, double distance = 5);

        ScreenItemEdge GetCloseEdge(IScreen screen, Point point, double distance = 5);
    }
}
