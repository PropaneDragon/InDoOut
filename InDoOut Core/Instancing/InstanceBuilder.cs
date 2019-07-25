using System;

namespace InDoOut_Core.Instancing
{
    /// <summary>
    /// Builds instances of type <typeparamref name="T"/> and subclasses.
    /// </summary>
    /// <typeparam name="T">The base class to build from.</typeparam>
    public class InstanceBuilder<T> : IInstanceBuilder<T> where T : class
    {
        /// <summary>
        /// Builds an instance of the given type. The type has to be inherited from <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">The type to build.</param>
        /// <returns>The requested type <paramref name="type"/> as type <typeparamref name="T"/>, or null if it has failed.</returns>
        public T BuildInstance(Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                try
                {
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        var instance = constructor.Invoke(new object[] { });
                        return instance as T;
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Builds an instance of the given type <typeparamref name="InstanceOf"/>. The type has to be inherited from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="InstanceOf">The type to build.</typeparam>
        /// <returns>The requested type <typeparamref name="InstanceOf"/> as <typeparamref name="T"/>, or null if it has failed.</returns>
        public T BuildInstance<InstanceOf>() where InstanceOf : class, T
        {
            return BuildInstance(typeof(InstanceOf));
        }
    }
}
