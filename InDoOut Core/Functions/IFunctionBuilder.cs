using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Instancing;

namespace InDoOut_Core.Functions
{
    /// <summary>
    /// Represents a builder specifically for building <see cref="IFunction"/> classes.
    /// </summary>
    public interface IFunctionBuilder : IInstanceBuilder<IFunction>
    {
    }
}
