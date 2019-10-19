using System.Linq;
using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Logging;
using InDoOut_Core.Threading.Safety;
using InDoOut_Core.Variables;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Properties are values that change how a <see cref="Function"/> operates, or passes information into a
    /// function to be calculated. These can be set by the user, or automatically set by the <see cref="AssociatedVariable"/>.
    /// </summary>
    public class Property<T> : InteractiveEntity<IFunction, IResult>, IProperty<T>
    {
        private readonly Value _value = new Value();

        /// <summary>
        /// A safe way of getting the <see cref="Description"/> of a property without exceptions.
        /// </summary>
        public string SafeDescription => TryGet.ValueOrDefault(() => Description);

        /// <summary>
        /// The variable associated with this property. If set to anything other than null it will use the
        /// value of the variable as the <see cref="RawComputedValue"/>, rather than using <see cref="RawValue"/>.
        /// </summary>
        public IVariable AssociatedVariable { get; set; } = null;

        /// <summary>
        /// The parent this property belongs to.
        /// </summary>
        public IFunction Parent => Connections.FirstOrDefault();

        /// <summary>
        /// The full computed value of the property. If <see cref="AssociatedVariable"/> is set it will use the
        /// value assigned to the variable, rather than <see cref="RawValue"/>.
        /// </summary>
        public string RawComputedValue => TryGet.ValueOrDefault(() => AssociatedVariable?.RawValue ?? _value.RawValue);

        /// <summary>
        /// Whether or not this is a required value for the function to operate.
        /// </summary>
        public bool Required { get; } = false;

        /// <summary>
        /// The description of what this property does.
        /// </summary>
        public string Description { get; private set; } = "";

        /// <summary>
        /// The value of this property, as the given type <typeparamref name="T"/>. This is similar to <see cref="RawValue"/>,
        /// but is automatically converted to the type of this property.
        /// <br/>
        /// <br/>
        /// <b>WARNING:</b> Consider <see cref="FullValue"/> or <see cref="RawComputedValue"/> instead for getting the value of the property,
        /// as those will include values from any connected <see cref="IResult"/>s whereas this won't, which could cause unintended side effects.
        /// </summary>
        public T BasicValue { get => TryGet.ValueOrDefault(() => _value.ConvertFromString<T>(RawValue)); set => RawValue = TryGet.ValueOrDefault(() => _value.ConvertToString(value)); }

        /// <summary>
        /// The full computed value of the property as type <typeparamref name="T"/>. If <see cref="AssociatedVariable"/> is set it will use the
        /// value assigned to the variable, rather than <see cref="BasicValue"/>.
        /// </summary>
        public T FullValue => TryGet.ValueOrDefault(() => _value.ConvertFromString<T>(RawComputedValue));

        /// <summary>
        /// Returns whether the current value is valid.
        /// </summary>
        public bool ValidValue => _value.ValidValue;

        /// <summary>
        /// Gets the raw, unprocessed value of this property.
        /// <br/>
        /// <br/>
        /// <b>WARNING:</b> Consider <see cref="FullValue"/> or <see cref="RawComputedValue"/> instead for getting the value of the property,
        /// as those will include values from any connected <see cref="IResult"/>s whereas this won't, which could cause unintended side effects.
        /// </summary>
        public string RawValue { get => _value.RawValue; set => _value.RawValue = value; }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a basic property with a name, description and optional requirement and initial value.
        /// </summary>
        /// <param name="name">The name of this property. This is visible to the user.</param>
        /// <param name="description">The description of this property. This is visible to the user.</param>
        /// <param name="required">Whether or not this property is required for the function to run.</param>
        /// <param name="initialValue">The initial value of the property.</param>
        public Property(string name, string description, bool required = false, T initialValue = default)
        {
            Name = name;
            Description = description;
            Required = required;
            BasicValue = initialValue;
        }

        /// <summary>
        /// Connects the <see cref="Property{T}"/> to a <see cref="IFunction"/>.
        /// </summary>
        /// <param name="function">The function to connect to.</param>
        /// <returns>Whether the connection was successful.</returns>
        public bool Connect(IFunction function)
        {
            return AddConnection(function);
        }

        /// <summary>
        /// Sets the current value from the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="Type">The type to convert from.</typeparam>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public bool ValueFrom<Type>(Type value) => _value.ValueFrom<Type>(value);

        /// <summary>
        /// Gets the <see cref="RawValue"/>, or <paramref name="defaultValue"/> if null.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string ValueOrDefault(string defaultValue = "") => _value.ValueOrDefault(defaultValue);

        /// <summary>
        /// Returns the value as the given <typeparamref name="Type"/>.
        /// </summary>
        /// <typeparam name="Type">The type to convert to.</typeparam>
        /// <param name="defaultValue">On failure, default to this value.</param>
        /// <returns>The value as <typeparamref name="Type"/>, or <paramref name="defaultValue"/> if failed.</returns>
        public Type ValueAs<Type>(Type defaultValue = default) => _value.ValueAs(defaultValue);

        /// <summary>
        /// A string representation of this entity.
        /// </summary>
        /// <returns>A string representation of this entity.</returns>
        public override string ToString()
        {
            return $"[PROPERTY {base.ToString()} [Name: {Name ?? "null"}] [Computed value: {RawComputedValue ?? "null"}] [Raw value: {RawValue ?? "null"}]]";
        }

        /// <summary>
        /// Processes the property fro the <see cref="IResult"/> that triggered it.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IResult"/> that triggerd this.</param>
        protected override void Process(IResult triggeredBy)
        {
            Log.Instance.Info($"Processing {this}");

            if (triggeredBy != null && !string.IsNullOrEmpty(triggeredBy.VariableName))
            {
                var variable = Connections.Select(function => function?.VariableStore?.GetVariable(triggeredBy.VariableName)).Where(variable => variable != null).FirstOrDefault();
                AssociatedVariable = variable;

                Log.Instance.Info($"Applying variable from ", triggeredBy, $" to {this}");
            }

            Log.Instance.Info($"Processed {this}");
        }
    }
}
