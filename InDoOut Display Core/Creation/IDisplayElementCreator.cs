using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using InDoOut_UI_Common.Creation;

namespace InDoOut_Display_Core.Creation
{
    /// <summary>
    /// A creator for handling the creation of <see cref="IDisplayElement"/> elements on a display.
    /// </summary>
    public interface IDisplayElementCreator : IElementCreator
    {
        /// <summary>
        /// Creates a container from an <see cref="IElementFunction"/>.
        /// </summary>
        /// <param name="elementFunction">The element function to create a container from.</param>
        /// <param name="setSizeFromMetadata">Whether or not to set the container size parameters from the stored metadata.</param>
        /// <returns></returns>
        IDisplayElementContainer Create(IElementFunction elementFunction, bool setSizeFromMetadata = true);

        /// <summary>
        /// Creates a container for a <see cref="IDisplayElement"/>.
        /// </summary>
        /// <param name="displayElement">The display element to create a container for.</param>
        /// <param name="setSizeFromMetadata">Whether or not to set the container size parameters from the stored metadata.</param>
        /// <returns></returns>
        IDisplayElementContainer Create(IDisplayElement displayElement, bool setSizeFromMetadata = true);
    }
}
