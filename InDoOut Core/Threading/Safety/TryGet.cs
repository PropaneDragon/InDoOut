using System;

namespace InDoOut_Core.Threading.Safety
{
    /// <summary>
    /// A collection of methods to safely get values.
    /// </summary>
    public static class TryGet
    {
        /// <summary>
        /// Gets a value from a function result unless an exception is thrown, where it returns
        /// a default value.
        /// </summary>
        /// <typeparam name="T">The value type that is returned.</typeparam>
        /// <param name="function">The function to process.</param>
        /// <param name="defaultValue">The default value to return on failure.</param>
        /// <returns></returns>
        public static T ValueOrDefault<T>(Func<T> function, T defaultValue = default(T))
        {
            if (function != null)
            {
                try
                {
                    return function.Invoke();
                }
                catch { }
            }

            return defaultValue;
        }
    }
}
