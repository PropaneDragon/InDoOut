using InDoOut_Core.Basic;
using InDoOut_Core.Variables;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Results are values that are set when a <see cref="Function"/> is run. These values
    /// can be named and will set a <see cref="Variables.IVariable"/> of that name when
    /// the function is finished. Those values can then be used on <see cref="IProperty"/>
    /// values.
    /// </summary>
    public class Result : NamedValue, IResult
    {
        private object _variableNameLock = new object();

        private string _variableName = null;

        /// <summary>
        /// Whether this result contains a valid value, in order to set a corrisponding
        /// <see cref="IVariable"/>.
        /// </summary>
        public bool IsSet => !string.IsNullOrEmpty(RawValue);

        /// <summary>
        /// A description of what this result represents.
        /// </summary>
        public string Description { get; private set; } = "";

        /// <summary>
        /// The name of the variable that will be set when the associated <see cref="IFunction"/>
        /// completes.
        /// </summary>
        public string VariableName
        {
            get { lock (_variableNameLock) return _variableName; }
            set { lock (_variableNameLock) _variableName = value; }
        }

        /// <summary>
        /// Creates a result with a name, description and optional defaultValue.
        /// </summary>
        /// <param name="name">The name of this result. This is visible to the user.</param>
        /// <param name="description">The description of this result. This is visible to the user.</param>
        /// <param name="defaultValue">The default value to set on this result.</param>
        public Result(string name, string description, string defaultValue = "")
        {
            Name = name;
            Description = description;
            RawValue = defaultValue;
        }

        /// <summary>
        /// Sets a variable with a name of the given <see cref="VariableName"/> and value of
        /// <see cref="NamedValue.RawValue"/> inside the given <paramref name="variableStore"/>.
        /// </summary>
        /// <param name="variableStore">A variable store to create/update the variable.</param>
        /// <returns>Whether the variable was set or not.</returns>
        public bool SetVariable(IVariableStore variableStore)
        {
            if (variableStore != null)
            {
                return variableStore.SetVariable(VariableName, RawValue);
            }

            return false;
        }

        /// <summary>
        /// Sets a variable's value directly from the <see cref="NamedValue.RawValue"/> without
        /// accounting for the variable name. It is recommended to use <see cref="SetVariable(IVariableStore)"/>
        /// instead, as it takes care of creating/updating the correct variable.
        /// </summary>
        /// <param name="variable">The variable to set.</param>
        /// <returns>Whether the variable was set or not.</returns>
        public bool SetVariable(IVariable variable)
        {
            if (variable != null)
            {
                variable.RawValue = RawValue;

                return true;
            }

            return false;
        }
    }
}
