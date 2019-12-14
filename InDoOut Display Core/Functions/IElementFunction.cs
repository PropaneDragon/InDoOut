using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Elements;

namespace InDoOut_Display_Core.Functions
{
    /// <summary>
    /// Represents a type of <see cref="IFunction"/> that can be used to update an interactive
    /// UI Element (<see cref="IDisplayElement"/>) on a screen.
    /// </summary>
    public interface IElementFunction : IFunction
    {
        /// <summary>
        /// Whether an update of the interface is required due to a change on the contained data.
        /// </summary>
        bool ShouldDisplayUpdate { get; }

        /// <summary>
        /// Creats a <see cref="IDisplayElement"/> which can be associated with this function.
        /// </summary>
        /// <returns>A <see cref="IDisplayElement"/> that can be associated with and update from this function.</returns>
        void PerformedUIUpdate();

        /// <summary>
        /// Should be called when an update on the UI has taken place, in order to
        /// reset <see cref="ShouldDisplayUpdate"/>.
        /// </summary>
        IDisplayElement CreateAssociatedUIElement();
    }
}
