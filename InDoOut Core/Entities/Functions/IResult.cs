using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents a result from a <see cref="IFunction"/>. Results are values given when the function has
    /// completed, and can be used to set <see cref="IProperty"/> values on other functions.
    /// </summary>
    public interface IResult : IOutputable, INamedEntity, IValue, ITriggerable<IFunction>, IConnectable<IProperty>
    {
        /// <summary>
        /// Whether the result has a value set on it or not.
        /// </summary>
        bool IsSet { get; }

        /// <summary>
        /// Connects this result to an <see cref="IProperty"/>.
        /// </summary>
        /// <param name="property">The property to connect to.</param>
        /// <returns>Whether the connection was successful.</returns>
        bool Connect(IProperty property);

        /// <summary>
        /// Disconnect this result from an <see cref="IProperty"/>.
        /// </summary>
        /// <param name="input">The <see cref="IProperty"/> to disconnect from.</param>
        /// <returns>Whether the property was disconnected.</returns>
        bool Disconnect(IProperty input);

        /// <summary>
        /// The description of what this result represents.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The name of the variable that will be set when the associated <see cref="IFunction"/>
        /// completes.
        /// </summary>
        string VariableName { get; set; }
    }
}
