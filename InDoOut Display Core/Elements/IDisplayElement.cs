using InDoOut_Display_Core.Functions;

namespace InDoOut_Display_Core.Elements
{
    /// <summary>
    /// Represents an element to be displayed on an interface.
    /// </summary>
    public interface IDisplayElement
    {
        /// <summary>
        /// The associated background function responsible for updating this
        /// element.
        /// </summary>
        IElementFunction AssociatedElementFunction { get; }
    }
}