using InDoOut_Core.Entities.Programs;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A function that is capable of running another <see cref="IProgram"/> internally. This allows
    /// for visibility into what the program is processing.
    /// </summary>
    public abstract class SelfRunnerFunction : Function, ISelfRunnerFunction
    {
        /// <summary>
        /// The currently loaded program.
        /// </summary>
        public abstract IProgram LoadedProgram { get; }
    }
}
