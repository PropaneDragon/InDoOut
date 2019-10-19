using InDoOut_Core.Entities.Programs;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents a function capable of running an <see cref="IProgram"/> within itself.
    /// </summary>
    public interface ISelfRunnerFunction : IFunction
    {
        /// <summary>
        /// The program currently loaded into the function.
        /// </summary>
        IProgram LoadedProgram { get; }
    }
}
