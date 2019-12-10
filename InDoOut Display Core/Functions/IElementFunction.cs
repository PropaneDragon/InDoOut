using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Elements;

namespace InDoOut_Display_Core.Functions
{
    /// <summary>
    /// Represents a type of <see cref="IFunction"/> that can be used to update an interactive
    /// UI Element (<see cref="IElement"/>) on a screen.
    /// </summary>
    public interface IElementFunction : IFunction
    {
        /// <summary>
        /// Creates an element that is associated with the function and can be updated
        /// from it.
        /// </summary>
        /// <returns>A new <see cref="IElement"/> that can be updated from this <see cref="IElementFunction"/>.</returns>
        IElement CreateAssociatedElement();
    }
}
