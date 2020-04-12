using InDoOut_Display_Core.Elements;

namespace InDoOut_Display_Core.Screens
{
    /// <summary>
    /// Represents a type of display that can host <see cref="IDisplayElement"/>s.
    /// </summary>
    public interface IDisplayElementDisplay
    {
        /// <summary>
        /// Adds a display element to this display.
        /// </summary>
        /// <param name="displayElement">The display element to add.</param>
        /// <returns>Whether or not the display element was added.</returns>
        bool AddDisplayElement(IDisplayElement displayElement);

        /// <summary>
        /// Removes a display element from this display.
        /// </summary>
        /// <param name="displayElement">The display element to remove.</param>
        /// <returns>Whether or not the display element was removed.</returns>
        bool RemoveDisplayElement(IDisplayElement displayElement);
    }
}
