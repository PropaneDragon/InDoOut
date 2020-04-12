using InDoOut_Display_Core.Screens;
using System.Windows;

namespace InDoOut_Display_Core.Actions.Resizing
{
    /// <summary>
    /// Represents an edge of an <see cref="IResizable"/>.
    /// </summary>
    public enum ResizeEdge
    {
        /// <summary>
        /// No edge
        /// </summary>
        None,
        /// <summary>
        /// Left section
        /// </summary>
        Left,
        /// <summary>
        /// Top section
        /// </summary>
        Top,
        /// <summary>
        /// Right section
        /// </summary>
        Right,
        /// <summary>
        /// Bottom section
        /// </summary>
        Bottom,
        /// <summary>
        /// Top left corner
        /// </summary>
        TopLeft,
        /// <summary>
        /// Top right corner
        /// </summary>
        TopRight,
        /// <summary>
        /// Bottom left corner
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Bottom right corner
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// Represents an object that can be resized from its edges.
    /// </summary>
    public interface IResizable
    {
        /// <summary>
        /// The size of this object.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Triggered when a resize event has started.
        /// </summary>
        /// <param name="screen">The screen where the resize event was started.</param>
        void ResizeStarted(IScreen screen);

        /// <summary>
        /// Triggered when a resize event has ended.
        /// </summary>
        /// <param name="screen">The screen where the resize event was ended.</param>
        void ResizeEnded(IScreen screen);

        /// <summary>
        /// Triggered when the resize edge has moved.
        /// </summary>
        /// <param name="screen">The screen where the edge was moved on.</param>
        /// <param name="edge">The edge that is currently being moved.</param>
        /// <param name="delta">The delta from the initial clicked point when resize was started.</param>
        void ResizeMoved(IScreen screen, ResizeEdge edge, Point delta);

        /// <summary>
        /// Whether or not the element can currently be resized.
        /// </summary>
        /// <param name="screen">The screen that is requesting a resize of this element.</param>
        /// <returns>Whether or not the element is allowed to resize on the given <paramref name="screen"/>.</returns>
        bool CanResize(IScreen screen);

        /// <summary>
        /// Whether or not the <paramref name="point"/> is close to any edge of this element.
        /// </summary>
        /// <param name="screen">The screen to check position against.</param>
        /// <param name="point">The position to check proximity against.</param>
        /// <param name="distance">A proximity distance to the edge.</param>
        /// <returns>Whether or not the given point is close to any edge of this resizable.</returns>
        bool CloseToEdge(IScreen screen, Point point, double distance = 5);

        /// <summary>
        /// Returns the closest edge to the element. If no edge is close, <see cref="ResizeEdge.None"/> is returned.
        /// </summary>
        /// <param name="screen">The screen to check position against.</param>
        /// <param name="point">The position to check proximity against.</param>
        /// <param name="distance">A proximity distance to the edge.</param>
        /// <returns>The closest edge to this resizable, or <see cref="ResizeEdge.None"/> if none is close.</returns>
        ResizeEdge GetCloseEdge(IScreen screen, Point point, double distance = 5);
    }
}
