namespace InDoOut_Core.Instancing
{
    /// <summary>
    /// Creates class singletons of type <typeparamref name="T"/>. Singletons have an <see cref="Instance"/> accessor that
    /// returns a singular instance of class <typeparamref name="T"/>. If it doesn't exist, it gets built.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton<T> where T : class
    {
        private static T _instance = null;

        /// <summary>
        /// An instance of <typeparamref name="T"/>.
        /// </summary>
        public static T Instance => GetOrCreateInstance();

        /// <summary>
        /// Ensures an instance has been built. Useful for if something needs to happen in the
        /// constructor.
        /// </summary>
        public static void EnsureInstanceBuilt() => _ = Instance;

        /// <summary>
        /// Creates and returns a brand new instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        protected static T CreateInstance()
        {
            var builder = new InstanceBuilder<T>();
            return builder.BuildInstance<T>();
        }

        private static T GetOrCreateInstance()
        {
            if (_instance == null)
            {
                _instance = CreateInstance();
            }

            return _instance;
        }
    }
}
