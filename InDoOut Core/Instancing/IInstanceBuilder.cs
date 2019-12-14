using System;

namespace InDoOut_Core.Instancing
{
    /// <summary>
    /// Represents a builder that can generate instances of classes of type <typeparamref name="T"/>.
    /// It can build all classes that can inherit from or are <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The base class to build from.</typeparam>
    public interface IInstanceBuilder<T> where T : class
    {
        /// <summary>
        /// Builds an instance of the given type. The type has to be inherited from <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">The type to build.</param>
        /// <returns>The requested type <paramref name="type"/> as type <typeparamref name="T"/>, or null if it has failed.</returns>
        T BuildInstance(Type type);

        /// <summary>
        /// Builds an instance of the given type with additional parameters. The type has to be inherited from <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">The type to build.</param>
        /// <param name="parameters">Additional parameters to pass to the constructor.</param>
        /// <returns>The requested type <paramref name="type"/> as type <typeparamref name="T"/>, or null if it has failed.</returns>
        T BuildInstance(Type type, params object[] parameters);

        /// <summary>
        /// Builds an instance of the given type <typeparamref name="InstanceOf"/>. The type has to be inherited from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="InstanceOf">The type to build.</typeparam>
        /// <returns>The requested type <typeparamref name="InstanceOf"/> as <typeparamref name="T"/>, or null if it has failed.</returns>
        T BuildInstance<InstanceOf>() where InstanceOf : class, T;

        /// <summary>
        /// Builds an instance of the given type with additional parameters. The type has to be inherited from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="InstanceOf">The type to build.</typeparam>
        /// <param name="parameters">Additional parameters to pass to the constructor.</param>
        /// <returns>The requested type <typeparamref name="InstanceOf"/> as <typeparamref name="T"/>, or null if it has failed.</returns>
        T BuildInstance<InstanceOf>(params object[] parameters) where InstanceOf : class, T;
    }
}
