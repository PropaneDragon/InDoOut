using InDoOut_Core.Basic;
using InDoOut_Core.Threading.Safety;
using InDoOut_Core.Variables;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Properties are values that change how a <see cref="Function"/> operates, or passes information into a
    /// function to be calculated. These can be set by the user, or automatically set by the <see cref="AssociatedVariable"/>.
    /// </summary>
    public class Property<T> : NamedValue, IProperty<T>
    {
        /// <summary>
        /// A safe way of getting the <see cref="Description"/> of a property without exceptions.
        /// </summary>
        public string SafeDescription => TryGet.ValueOrDefault(() => Description);

        /// <summary>
        /// The variable associated with this property. If set to anything other than null it will use the
        /// value of the variable as the <see cref="RawComputedValue"/>, rather than using <see cref="NamedValue.RawValue"/>.
        /// </summary>
        public IVariable AssociatedVariable { get; set; } = null;

        /// <summary>
        /// The full computed value of the property. If <see cref="AssociatedVariable"/> is set it will use the
        /// value assigned to the variable, rather than <see cref="NamedValue.RawValue"/>.
        /// </summary>
        public string RawComputedValue => TryGet.ValueOrDefault(() => AssociatedVariable != null ? AssociatedVariable.RawValue : RawValue);

        /// <summary>
        /// Whether or not this is a required value for the function to operate.
        /// </summary>
        public bool Required { get; } = false;

        /// <summary>
        /// The description of what this property does.
        /// </summary>
        public string Description { get; private set; } = "";

        /// <summary>
        /// The value of this property, as the given type <typeparamref name="T"/>. This is similar to <see cref="NamedValue.RawValue"/>,
        /// but is automatically converted to the type of this property.
        /// </summary>
        public T BasicValue { get => TryGet.ValueOrDefault(() => ConvertFromString<T>(RawValue)); set => RawValue = TryGet.ValueOrDefault(() => ConvertToString(value)); }

        /// <summary>
        /// The full computed value of the property as type <typeparamref name="T"/>. If <see cref="AssociatedVariable"/> is set it will use the
        /// value assigned to the variable, rather than <see cref="BasicValue"/>.
        /// </summary>
        public T FullValue { get => TryGet.ValueOrDefault(() => ConvertFromString<T>(RawComputedValue)); }

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
    }
}
