using InDoOut_Core.Options;
using InDoOut_Core.Options.Types;
using InDoOut_Core.Reporting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Represents a basic JSON shell for a list of <see cref="IOption"/>
    /// </summary>
    [JsonObject("optionGroup")]
    public class JsonOptions
    {
        [JsonProperty("options")]
        private List<JsonOption> Options { get; set; } = new List<JsonOption>();

        /// <summary>
        /// Creates a JSON container for a list of options for storage.
        /// </summary>
        /// <param name="optionHolder">The options to create a container for.</param>
        /// <returns>A container for storing options.</returns>
        public static JsonOptions CreateFromList(IOptionHolder optionHolder)
        {
            if (optionHolder != null)
            {
                var jsonOptions = new JsonOptions();

                foreach (var option in optionHolder.Options)
                {
                    if (option != null)
                    {
                        var jsonOption = JsonOption.CreateFromOption(option);

                        jsonOptions.Options.Add(jsonOption);
                    }
                }

                return jsonOptions;
            }

            return null;
        }

        /// <summary>
        /// Sets the values in a list of options to the stored values.
        /// <para />
        /// Note that values will only get set on options with matching names in the stored
        /// options.
        /// </summary>
        /// <param name="optionHolder">A list of options to apply values to.</param>
        /// <returns>Whether or not all options were set correctly.</returns>
        public List<IFailureReport> SetListValues(IOptionHolder optionHolder)
        {
            var failures = new List<IFailureReport>();

            if (optionHolder != null)
            {
                var allSuccessful = true;

                foreach (var option in Options)
                {
                    var foundOption = optionHolder.Options.FirstOrDefault(internalOption => internalOption.Name == option.Name);

#pragma warning disable IDE0045 // Convert to conditional expression
                    if (foundOption != null)
                    {
                        allSuccessful = option.Set(foundOption) && allSuccessful;
                    }
                    else
                    {
                        allSuccessful = optionHolder.AddUnregisteredOption(new HiddenStringOption(option.Name, defaultValue: option.Value)) && allSuccessful;
                    }
#pragma warning restore IDE0045 // Convert to conditional expression
                }
            }
            else
            {
                failures.Add(new FailureReport(0, "The provided options container was invalid.", true));
            }

            return failures;
        }
    }
}
