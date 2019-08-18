using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Instancing;

namespace InDoOut_Core.Functions
{
    /// <summary>
    /// A builder specifically for building classes and subclasses of <see cref="IFunction"/>.
    /// </summary>
    public class FunctionBuilder : InstanceBuilder<IFunction>, IFunctionBuilder
    {
    }
}
