namespace InDoOut_Core.Instancing
{
    /// <summary>
    /// Represents a singleton. Singletons have an <see cref="Instance"/> accessor that
    /// returns a singular instance of a class. If it doesn't exist, it gets built.
    /// </summary>
    public interface ISingleton<T> where T : class
    {
        /// <summary>
        /// An instance of <typeparamref name="T"/>.
        /// </summary>
        public static T Instance { get; }
    }
}
