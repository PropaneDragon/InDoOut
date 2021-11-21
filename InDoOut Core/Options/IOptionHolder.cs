using System.Collections.Generic;

namespace InDoOut_Core.Options
{
    /// <summary>
    /// Contains options that can be saved and loaded.
    /// </summary>
    public interface IOptionHolder
    {
        /// <summary>
        /// All stored options.
        /// </summary>
        List<IOption> Options { get; }

        /// <summary>
        /// Registers an option with the holder.
        /// </summary>
        /// <param name="option">The option to register.</param>
        /// <param name="setFromUnregisteredOptions">Allows the option value to be set from
        /// unregistered options if the names match. See <seealso cref="AddUnregisteredOption(IOption)"/>
        /// for more on unregistered options.</param>
        /// <returns>Whether the option was successfully registed.</returns>
        bool RegisterOption(IOption option, bool setFromUnregisteredOptions = true);

        /// <summary>
        /// Deregisters an option from the holder.
        /// </summary>
        /// <param name="option">The option to deregister.</param>
        /// <returns>Whether the option was deregistered successfully.</returns>
        bool DeregisterOption(IOption option);

        /// <summary>
        /// Updates an option from a stored value without permanently registering it.
        /// </summary>
        /// <param name="option">The option to update</param>
        /// <returns>Whether the option was successfully updated. If no value is present to set it will return false.</returns>
        bool UpdateOption(IOption option);

        /// <summary>
        /// Adds an option that isn't registered, but will set the value of a registered 
        /// option with the same name when added.
        /// <para/>
        /// This is useful for loading in options
        /// that aren't immediately registered, or loaded progressively over time.
        /// </summary>
        /// <param name="option">The option to add.</param>
        /// <returns>Whether the option was successfully added.</returns>
        bool AddUnregisteredOption(IOption option);
    }
}
