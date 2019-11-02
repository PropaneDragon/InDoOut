using InDoOut_Core.Basic;

namespace InDoOut_Core.Options
{
    /// <summary>
    /// Represents an option that can be saved, loaded and modified by the user.
    /// </summary>
    public interface IOption : IValue, INamed
    {
        /// <summary>
        /// Whether this option should be visible to the user or not.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// The description of the option to be displayed on the interface.
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Represents a specialised option that has <typeparamref name="ValueType"/> to convert between.
    /// </summary>
    /// <typeparam name="ValueType">The core value of this option.</typeparam>
    public interface IOption<ValueType> : IOption
    {
        /// <summary>
        /// The full value of this option.
        /// </summary>
        ValueType Value { get; set; }
    }
}
