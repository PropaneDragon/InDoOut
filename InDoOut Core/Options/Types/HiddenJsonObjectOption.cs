using Newtonsoft.Json;

namespace InDoOut_Core.Options.Types
{
    /// <summary>
    /// Converts an object to json for storage and back again.
    /// </summary>
    public class HiddenJsonObjectOption : Option<string>
    {
        /// <summary>
        /// Creates an option capable of storing an object via json.
        /// </summary>
        /// <param name="name">The option name</param>
        /// <param name="description">The option description</param>
        public HiddenJsonObjectOption(string name, string description = "") : base(name, description)
        {
        }

        /// <summary>
        /// Sets the object value from an object.
        /// </summary>
        /// <param name="object">The object to store.</param>
        /// <returns></returns>
        public bool FromObject(object @object)
        {
            try
            {
                Value = JsonConvert.SerializeObject(@object, Formatting.None);

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Converts the stored option to an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToObject<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(ValueOrDefault());
            }
            catch { }

            return default;
        }
    }
}
