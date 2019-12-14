using InDoOut_Core.Instancing;

namespace InDoOut_Display_Core.Functions
{
    /// <summary>
    /// Represents a function builder capable of creating <see cref="IElementFunction"/> instances, which
    /// host logic for interface elements.
    /// </summary>
    public interface IElementFunctionBuilder : IInstanceBuilder<IElementFunction>
    {
    }
}
