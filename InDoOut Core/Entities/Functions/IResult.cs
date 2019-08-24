using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Variables;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents a result from a <see cref="IFunction"/>. Results are values given when the function has
    /// completed, and can be used to set <see cref="Variables.IVariable"/> values which can then be
    /// used to set <see cref="IProperty"/> values on other functions.
    /// </summary>
    public interface IResult : IOutputable, INamedEntity, IValue, ITriggerable<IFunction>, IConnectable<IProperty>
    {
        /// <summary>
        /// Whether the result has a value set on it or not.
        /// </summary>
        bool IsSet { get; }

        /// <summary>
        /// Connects this result to a property.
        /// </summary>
        /// <param name="property">The property to connect to.</param>
        /// <returns>Whether the connection was successful.</returns>
        bool Connect(IProperty property);

        /// <summary>
        /// The description of what this result represents.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The name of the variable that will be set when the associated <see cref="IFunction"/>
        /// completes.
        /// </summary>
        string VariableName { get; set; }

        /// <summary>
        /// Sets a variable with a name of the given <see cref="VariableName"/> and value of
        /// <see cref="IValue.RawValue"/> inside the given <paramref name="variableStore"/>.
        /// </summary>
        /// <param name="variableStore">A variable store to create/update the variable.</param>
        /// <returns>Whether the variable was set or not.</returns>
        bool SetVariable(IVariableStore variableStore);

        /// <summary>
        /// Sets a variable's value directly from the <see cref="IValue.RawValue"/> without
        /// accounting for the variable name. It is recommended to use <see cref="SetVariable(IVariableStore)"/>
        /// instead, as it takes care of creating/updating the correct variable.
        /// </summary>
        /// <param name="variable">The variable to set.</param>
        /// <returns>Whether the variable was set or not.</returns>
        bool SetVariable(IVariable variable);
    }
}
