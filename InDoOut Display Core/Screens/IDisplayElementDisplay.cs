using InDoOut_Display_Core.Creation;
using InDoOut_Display_Core.Elements;

namespace InDoOut_Display_Core.Screens
{
    /// <summary>
    /// Represents a type of display that can host <see cref="IDisplayElement"/>s.
    /// </summary>
    public interface IDisplayElementDisplay
    {
        /// <summary>
        /// A <see cref="IDisplayElementCreator"/> to handle the creation of
        /// <see cref="IDisplayElement"/>s.
        /// </summary>
        IDisplayElementCreator DisplayElementCreator { get; }
    }
}
