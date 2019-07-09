using InDoOut_Core.Entities.Core;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// The type of output.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// An output from a good result.
        /// </summary>
        Positive,

        /// <summary>
        /// An output from a bad result.
        /// </summary>
        Negative,

        /// <summary>
        /// An output from neither a good nor bad result.
        /// </summary>
        Neutral
    }

    /// <summary>
    /// Represents an output that can be connected to any
    /// <see cref="IInput"/> entity and is triggered by a
    /// <see cref="IFunction"/> entity.
    /// </summary>
    public interface IOutput : INamedEntity, ITriggerable<IFunction>, IConnectable<IInput>
    {
        /// <summary>
        /// Connect this output to an <see cref="IInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="IInput"/> to connect to.</param>
        /// <returns>Whether a connection was made.</returns>
        bool Connect(IInput input);

        /// <summary>
        /// Disconnect this output from an <see cref="IInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="IInput"/> to disconnect from.</param>
        /// <returns>Whether the input was disconnected.</returns>
        bool Disconnect(IInput input);
    }
}
