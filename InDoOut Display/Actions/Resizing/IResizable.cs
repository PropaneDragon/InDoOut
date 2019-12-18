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
        Size Size { get; }

        void ResizeStarted(IScreen screen);
        void ResizeEnded(IScreen screen);
        void SetEdgeToMouse(IScreen screen, ResizeEdge edge);

        bool CanResize(IScreen screen);
        bool CloseToEdge(IScreen screen, Point point, double distance = 5);

        ResizeEdge GetCloseEdge(IScreen screen, Point point, double distance = 5);
    }
}
