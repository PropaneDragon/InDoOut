using InDoOut_Core.Entities.Programs;
using InDoOut_Display_Core.Elements;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display_Core.Screens
{
    /// <summary>
    /// Represents an edge of a <see cref="IScreen"/>
    /// </summary>
    public enum ScreenEdge
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
    /// Represents a screen that is capable of hosting <see cref="IDisplayElement"/> items.
    /// </summary>
    public interface IScreen : ICommonDisplay, IDisplayElementDisplay
    {
        /// <summary>
        /// The current view mode of this display.
        /// </summary>
        ProgramViewMode CurrentViewMode { get; set; }

        /// <summary>
        /// The program associated with this display.
        /// </summary>
        IProgram AssociatedProgram { get; set; }

        /// <summary>
        /// The parent screen that holds base connections.
        /// </summary>
        IScreenConnections AssociatedScreenConnections { get; set; }

        /// <summary>
        /// Returns the closest edge to the element. If no edge is close, <see cref="ScreenEdge.None"/> is returned.
        /// </summary>
        /// <param name="point">The position to check proximity against.</param>
        /// <param name="distance">A proximity distance to the edge.</param>
        /// <returns>The closest edge to this screen, or <see cref="ScreenEdge.None"/> if none is close.</returns>
        ScreenEdge GetCloseEdge(Point point, double distance = 5);

        /// <summary>
        /// Clears the screen of all elements.
        /// </summary>
        /// <returns>Whether it successfully cleared elements.</returns>
        bool Clear();

        /// <summary>
        /// Whether or not the <paramref name="point"/> is close to any edge of this element.
        /// </summary>
        /// <param name="point">The position to check proximity against.</param>
        /// <param name="distance">A proximity distance to the edge.</param>
        /// <returns>Whether or not the given point is close to any edge of this screen.</returns>
        bool PointCloseToScreenItemEdge(Point point, double distance = 5);
    }
}