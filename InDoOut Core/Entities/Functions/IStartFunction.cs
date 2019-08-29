using System.Collections.Generic;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents a function that is called automatically when the <see cref="Programs.IProgram"/> is started.
    /// </summary>
    public interface IStartFunction : IFunction
    {
        /// <summary>
        /// A list of results that are passed through from another program, or the command line.
        /// </summary>
        List<IResult> PassthroughResults { get; }
    }
}
