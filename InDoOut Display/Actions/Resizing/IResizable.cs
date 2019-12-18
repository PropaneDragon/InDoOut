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
        Size Size { get; set; }

        void ResizeStarted(IScreen screen);
        void ResizeEnded(IScreen screen);
        void SetEdgeDistance(IScreen screen, ResizeEdge edge, double distance);

        bool CanResize(IScreen screen);
        bool CloseToEdge(IScreen screen, Point point, double distance = 5);

        ResizeEdge GetCloseEdge(IScreen screen, Point point, double distance = 5);
    }
}
