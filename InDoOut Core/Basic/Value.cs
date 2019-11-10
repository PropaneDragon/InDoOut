using InDoOut_Core.Threading.Safety;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace InDoOut_Core.Basic
{
    /// <summary>
    /// A basic value, along with some conversion utilities.
    /// </summary>
    public class Value : IValue
    {
        private readonly object _rawValueLock = new object();
        private string _rawValue = "";
        
        /// <summary>
        /// An event that gets fired when the value changes.
        /// <para/>
        /// Note: Not thread safe. This spawns a new thread every time the value changes.
        /// </summary>
        public event EventHandler<ValueChangedEvent> OnValueChanged;

        /// <summary>
        /// Whether it has a valid name and value.
        /// </summary>
        public bool ValidValue => RawValue != null;

        /// <summary>
        /// The value associated with the name.
        /// </summary>
        public string RawValue
        {
            get { lock (_rawValueLock) { return _rawValue; } }
            set { lock (_rawValueLock) { UpdateRawValueAndAnnounce(value); } }
        }

        /// <summary>
        /// Returns <see cref="RawValue"/>, or <paramref name="defaultValue"/> if null.
        /// </summary>
        /// <param name="defaultValue">The value to return if the stored value is null.</param>
        /// <returns>The value, or <paramref name="defaultValue"/> if null.</returns>
        public string ValueOrDefault(string defaultValue = "") => TryGet.ValueOrDefault(() => RawValue) ?? defaultValue;

        /// <summary>
        /// Converts the <see cref="RawValue"/> to the given type <typeparamref name="T"/>. If this conversion
        /// fails, <paramref name="defaultValue"/> is returned instead.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <returns>The converted value or <paramref name="defaultValue"/> if conversion fails.</returns>
        public T ValueAs<T>(T defaultValue = default) => TryGet.ValueOrDefault(() => ConvertFromString<T>(RawValue), defaultValue);

        /// <summary>
        /// Sets the value from the given <typeparamref name="T"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the value being given.</typeparam>
        /// <param name="value">The value to be set.</param>
        /// <returns>Whether the value was converted and set.</returns>
        public bool ValueFrom<T>(T value) => TryGet.ExecuteOrFail(() => RawValue = ConvertToString(value));

        /// <summary>
        /// Converts a value from a string to the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="value">The value to convert to the type <typeparamref name="T"/>.</param>
        /// <returns>The value of the string as the type <typeparamref name="T"/>.</returns>
        public T ConvertFromString<T>(string value)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        /// <summary>
        /// Converts a value of type <typeparamref name="T"/> to a string.
        /// </summary>
        /// <typeparam name="T">The type to convert from.</typeparam>
        /// <param name="value">The value of the type to convert to a string.</param>
        /// <returns>A string representation of the value given.</returns>
        public string ConvertToString<T>(T value)
        {
            if (value == null)
            {
                return default(T)?.ToString();
            }

            return TypeDescriptor.GetConverter(typeof(T)).ConvertToString(value);
        }

        /// <summary>
        /// Gets the string representation of this value.
        /// </summary>
        /// <returns>The string representation of this value.</returns>
        public override string ToString()
        {
            return $"[Valid: {ValidValue}] [Value: {RawValue}]";
        }

        private void UpdateRawValueAndAnnounce(string value)
        {
            var oldValue = _rawValue;
            _rawValue = value;

            if (oldValue != value)
            {
                _ = Task.Run(() => OnValueChanged?.Invoke(this, new ValueChangedEvent(this)));
            }
        }
    }
}
