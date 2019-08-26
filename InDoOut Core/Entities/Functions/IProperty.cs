using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Variables;
using System;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents a property, which are values that can be applied to
    /// <see cref="IFunction"/>s to alter their behaviour, or provide
    /// required input values.
    /// </summary>
    public interface IProperty : IInputable, INamedEntity, IValue, ITriggerable<IResult>, IConnectable<IFunction>
    {
        /// <summary>
        /// Whether or not this property is required for the function to work.
        /// </summary>
        bool Required { get; }

        /// <summary>
        /// Connects the property to a function.
        /// </summary>
        /// <param name="function">The function to connect to.</param>
        /// <returns>Whether the connection was successful.</returns>
        bool Connect(IFunction function);

        /// <summary>
        /// The description of what this property does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// A safer way of getting <see cref="Description"/> without exceptions.
        /// </summary>
        string SafeDescription { get; }

        /// <summary>
        /// Computes the value based off of whether <see cref="AssociatedVariable"/> is set or
        /// not. If it is set, it will use the <see cref="AssociatedVariable"/> for the value,
        /// or otherwise it just uses <see cref="IValue.RawValue"/>.
        /// </summary>
        string RawComputedValue { get; }

        /// <summary>
        /// A variable that is associated with this property. This automatically sets
        /// the value of the property to that of the variable when the value is requested.
        /// </summary>
        IVariable AssociatedVariable { get; set; }

        /// <summary>
        /// The parent of this input.
        /// </summary>
        IFunction Parent { get; }
    }

    /// <summary>
    /// Represents a specific type of property that has a value automatically
    /// converted to and from type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to convert to and from.</typeparam>
    public interface IProperty<T> : IProperty
    {
        /// <summary>
        /// The value of this property.
        /// </summary>
        [Obsolete("RawValue does not contain the full computed value from any Results linked to the Property. Perhaps you meant to use FullValue instead? If this is intended, please ignore.")]
        T BasicValue { get; set; }

        /// <summary>
        /// Computes the value based off of whether <see cref="IProperty.AssociatedVariable"/> is set or
        /// not. If it is set, it will use the <see cref="IProperty.AssociatedVariable"/> for the value,
        /// or otherwise it just uses <see cref="BasicValue"/>.
        /// </summary>
        T FullValue { get; }
    }
}
