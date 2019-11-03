using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// A generic class for storing and loading JSON data to the disk.
    /// </summary>
    public static class GenericJsonStorer
    {
        /// <summary>
        /// Saves <typeparamref name="JsonType"/> to the given <paramref name="path"/>.
        /// </summary>
        /// <param name="jsonType">The JSON to save.</param>
        /// <param name="path">The path to save to.</param>
        /// <returns>Whether or not the JSON data could be saved to the path.</returns>
        public static List<IFailureReport> Save<JsonType>(JsonType jsonType, string path) where JsonType : class
        {
            var failures = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(path) && jsonType != null)
            {
                try
                {
                    var jsonText = JsonConvert.SerializeObject(jsonType, Formatting.Indented);

                    File.WriteAllText(path, jsonText);

                    if (!File.Exists(path))
                    {
                        failures.Add(new FailureReport((int)SaveResult.InsufficientPermissions, $"The file could not be saved to the given path ({path})", true));
                    }
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is DirectoryNotFoundException)
                {
                    failures.Add(new FailureReport((int)SaveResult.InvalidLocation, $"The location given is invalid ({path})", true));
                }
                catch (UnauthorizedAccessException)
                {
                    failures.Add(new FailureReport((int)SaveResult.InsufficientPermissions, $"You don't have access to the file path given ({path})", true));
                }
                catch
                {
                    failures.Add(new FailureReport((int)SaveResult.Unknown, $"There was an unknown error while trying to serialise the item to be saved.", true));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)SaveResult.InvalidLocation, $"Invalid file location given ({path})", true));
            }

            return failures;
        }

        /// <summary>
        /// Loads JSON shell data as type <typeparamref name="JsonType"/> from the given <paramref name="path"/>.
        /// </summary>
        /// <returns>JSON data from the path, or null if failed.</returns>
        public static JsonType Load<JsonType>(string path) where JsonType : class
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var fileText = File.ReadAllText(path);
                var jsonOptions = JsonConvert.DeserializeObject<JsonType>(fileText);

                return jsonOptions;
            }

            return null;
        }
    }
}
