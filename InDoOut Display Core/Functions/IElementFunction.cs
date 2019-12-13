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
        bool ShouldDisplayUpdate { get; }

        void PerformedUIUpdate();

        IDisplayElement CreateAssociatedUIElement();
    }
}
