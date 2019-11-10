using System;

namespace InDoOut_Core.Basic
{
    /// <summary>
    /// Represents an event triggered when the stored value is changed.
    /// </summary>
    public class ValueChangedEvent : EventArgs
    {
        /// <summary>
        /// The value that has changed.
        /// </summary>
        public IValue Value { get; private set; } = null;

        private ValueChangedEvent()
        {
        }

        /// <summary>
        /// Creates a basic event with a given value.
        /// </summary>
        /// <param name="value">The value that has changed.</param>
        public ValueChangedEvent(IValue value) : this()
        {
            Value = value;
        }
    }
}
