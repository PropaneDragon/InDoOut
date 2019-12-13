using InDoOut_Display_Core.Functions;

namespace InDoOut_Display_Core.Elements
{
    public interface IDisplayElement
    {
        IElementFunction AssociatedElement { get; }
    }
}