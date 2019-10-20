using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Logging;
using InDoOut_Core.Variables;
using System;
using System.Linq;
using System.Threading;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Results are values that are set when a <see cref="Function"/> is run. These values
    /// can be named and will set a <see cref="IVariable"/> of that name when
    /// the function is finished. Those values can then be used on <see cref="IProperty"/>
    /// values.
    /// </summary>
    public class Result : InteractiveEntity<IProperty, IFunction>, IResult
    {
        private readonly object _variableNameLock = new object();

        private string _variableName = Guid.NewGuid().ToString();

        private readonly Value _value = new Value();

        /// <summary>
        /// Whether this result contains a valid value, in order to set a corrisponding
        /// <see cref="IVariable"/>.
        /// </summary>
        public bool IsSet => !string.IsNullOrEmpty(_value.RawValue);

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
        /// The name of the result.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether the value of this result is valid.
        /// </summary>
        public bool ValidValue => _value.ValidValue;

        /// <summary>
        /// The raw value of this property.
        /// </summary>
        public string RawValue { get => _value.RawValue; set => _value.RawValue = value; }

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
        /// Connects this result to a <see cref="IProperty"/>.
        /// </summary>
        /// <param name="property">The property to connect to.</param>
        /// <returns>Whether the connection was a success.</returns>
        public bool Connect(IProperty property)
        {
            return AddConnection(property);
        }

        /// <summary>
        /// Disconnects this result from a <see cref="IProperty"/>.
        /// </summary>
        /// <param name="property">The property to disconnect from.</param>
        /// <returns>Whether the disconnection was a success.</returns>
        public bool Disconnect(IProperty property)
        {
            return RemoveConnection(property);
        }

        /// <summary>
        /// Sets a variable with a name of the given <see cref="VariableName"/> and value of
        /// <see cref="RawValue"/> inside the given <paramref name="variableStore"/>.
        /// </summary>
        /// <param name="variableStore">A variable store to create/update the variable.</param>
        /// <returns>Whether the variable was set or not.</returns>
        public bool SetVariable(IVariableStore variableStore)
        {
            return variableStore != null ? variableStore.SetVariable(VariableName, _value.RawValue) : false;
        }

        /// <summary>
        /// Sets a variable's value directly from the <see cref="RawValue"/> without
        /// accounting for the variable name. It is recommended to use <see cref="SetVariable(IVariableStore)"/>
        /// instead, as it takes care of creating/updating the correct variable.
        /// </summary>
        /// <param name="variable">The variable to set.</param>
        /// <returns>Whether the variable was set or not.</returns>
        public bool SetVariable(IVariable variable)
        {
            if (variable != null)
            {
                variable.RawValue = _value.RawValue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the value from the given <paramref name="value"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert from.</typeparam>
        /// <param name="value">The value to store.</param>
        /// <returns>Whether the value has been set.</returns>
        public bool ValueFrom<T>(T value) => _value.ValueFrom(value);

        /// <summary>
        /// Returns the <see cref="RawValue"/>, or <paramref name="defaultValue"/> if null.
        /// </summary>
        /// <param name="defaultValue">The default value to return on failure.</param>
        /// <returns>The value of this result.</returns>
        public string ValueOrDefault(string defaultValue = "") => _value.ValueOrDefault(defaultValue);

        /// <summary>
        /// Returns the value converted to type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="defaultValue">The default value to return if converison is not possible.</param>
        /// <returns>The <see cref="RawValue"/> as type <typeparamref name="T"/>, or <paramref name="defaultValue"/> if conversion was not possible.</returns>
        public T ValueAs<T>(T defaultValue = default) => _value.ValueAs(defaultValue);

        /// <summary>
        /// A string representation of this entity.
        /// </summary>
        /// <returns>A string representation of this entity.</returns>
        public override string ToString()
        {
            return $"[RESULT {base.ToString()} [Name: {Name ?? "null"}] [Value: {RawValue ?? "null"}]]";
        }

        /// <summary>
        /// Processes the result from the function that triggered it.
        /// </summary>
        /// <param name="triggeredBy">The function that triggered this result.</param>
        protected override void Process(IFunction triggeredBy)
        {
            Log.Instance.Info($"Processing {this}");

            if (SetVariable(triggeredBy?.VariableStore))
            {
                Log.Instance.Info($"Applied variable value: {this}");
            }
            else
            {
                Log.Instance.Error($"Failed to apply variable value for {this}");
            }

            foreach (var connection in Connections)
            {
                Log.Instance.Info($"Setting: {this} ========== ", connection);

                connection.Trigger(this);
            }

            while (Connections.Any(connection => connection.Running))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Log.Instance.Info($"Processed {this}");
        }
    }
}
