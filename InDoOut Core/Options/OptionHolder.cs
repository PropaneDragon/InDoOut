using InDoOut_Core.Logging;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Core.Options
{
    /// <summary>
    /// A container for stored options.
    /// </summary>
    public class OptionHolder : IOptionHolder
    {
        private readonly List<IOption> _unregisteredOptions = new List<IOption>();

        /// <summary>
        /// All options currently registered with the holder.
        /// </summary>
        public List<IOption> Options { get; private set; } = new List<IOption>();

        /// <summary>
        /// Registers an option with the holder.
        /// </summary>
        /// <param name="option">The option to register.</param>
        /// <param name="setFromUnregisteredOptions">Allows the option value to be set from
        /// unregistered options if the names match. See <seealso cref="AddUnregisteredOption(IOption)"/>
        /// for more on unregistered options.</param>
        /// <returns>Whether the option was successfully registed.</returns>
        public bool RegisterOption(IOption option, bool setFromUnregisteredOptions = true)
        {
            Log.Instance.Info("Registering option: ", option);

            if (option != null && !string.IsNullOrEmpty(option.Name) && !Options.Contains(option))
            {
                Options.Add(option);

                if (setFromUnregisteredOptions)
                {
                    var foundUnregisteredOption = _unregisteredOptions.FirstOrDefault(unregisteredOption => unregisteredOption.Name == option.Name);
                    if (foundUnregisteredOption != null)
                    {
                        Log.Instance.Info("Applying option value from unregistered option: ", option, " to ", foundUnregisteredOption);

                        option.RawValue = foundUnregisteredOption.RawValue;
                    }
                }

                Log.Instance.Info("Registered option: ", option);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Deregisters an option from the holder.
        /// </summary>
        /// <param name="option">The option to deregister.</param>
        /// <returns>Whether the option was deregistered successfully.</returns>
        public bool DeregisterOption(IOption option)
        {
            Log.Instance.Info("Deregistering option: ", option);

            if (option != null && Options.Contains(option))
            {
                _ = Options.Remove(option);

                Log.Instance.Info("Deregistered option: ", option);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an option that isn't registered, but will set the value of a registered 
        /// option with the same name when added.
        /// <para/>
        /// This is useful for loading in options
        /// that aren't immediately registered, or loaded progressively over time.
        /// </summary>
        /// <param name="option">The option to add.</param>
        /// <returns>Whether the option was successfully added.</returns>
        public bool AddUnregisteredOption(IOption option)
        {
            Log.Instance.Info("Adding unregistered option: ", option);

            if (option != null && !string.IsNullOrEmpty(option.Name) && !_unregisteredOptions.Contains(option))
            {
                _unregisteredOptions.Add(option);

                Log.Instance.Info("Added unregistered option: ", option);

                return true;
            }

            return false;
        }
    }
}
