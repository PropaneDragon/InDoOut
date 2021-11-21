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
            try
            {
                using var fileStream = new FileStream(path, FileMode.Create);

                return Save(jsonType, fileStream);
            }
            catch
            {
                return new List<IFailureReport>() { new FailureReport((int)SaveResult.InsufficientPermissions, "The file could not be created at the given location.") };
            }
        }

        /// <summary>
        /// Saves <typeparamref name="JsonType"/> to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="jsonType">The JSON to save.</param>
        /// <param name="stream">The stream to save to.</param>
        /// <returns>Whether or not the JSON data could be saved to the stream.</returns>
        public static List<IFailureReport> Save<JsonType>(JsonType jsonType, Stream stream) where JsonType : class
        {
            var failures = new List<IFailureReport>();

            if ((stream?.CanWrite ?? false) && jsonType != null)
            {
                try
                {
                    using var writer = new StreamWriter(stream, leaveOpen: true);

                    var jsonText = JsonConvert.SerializeObject(jsonType, Formatting.Indented);
                    if (jsonText != null)
                    {
                        writer.Write(jsonText);
                    }
                    else
                    {
                        failures.Add(new FailureReport((int)SaveResult.InsufficientPermissions, $"The entity given couldn't be exported to json.", true));
                    }

                    stream.Flush();
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is DirectoryNotFoundException)
                {
                    failures.Add(new FailureReport((int)SaveResult.InvalidLocation, $"The data given couldn't be written to the chosen location.", true));
                }
                catch (UnauthorizedAccessException)
                {
                    failures.Add(new FailureReport((int)SaveResult.InsufficientPermissions, $"Permission couldn't be given to write to the chosen location.", true));
                }
                catch
                {
                    failures.Add(new FailureReport((int)SaveResult.Unknown, $"There was an unknown error while trying to serialise the item to be saved.", true));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)SaveResult.InvalidLocation, $"The location or device given cannot be written to.", true));
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
                try
                {
                    using var fileStream = new FileStream(path, FileMode.Open);

                    return Load<JsonType>(fileStream);
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Loads JSON shell data as type <typeparamref name="JsonType"/> from the given <paramref name="stream"/>.
        /// </summary>
        /// <returns>JSON data from the path, or null if failed.</returns>
        public static JsonType Load<JsonType>(Stream stream) where JsonType : class
        {
            if (stream?.CanRead ?? false)
            {
                try
                {
                    if (stream.CanSeek)
                    {
                        _ = stream.Seek(0, SeekOrigin.Begin);
                    }

                    using var reader = new StreamReader(stream, leaveOpen: true);
                    var fileText = reader.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(fileText))
                    {
                        var jsonOptions = JsonConvert.DeserializeObject<JsonType>(fileText);
                        return jsonOptions;
                    }

                    stream.Flush();
                }
                catch { }
            }

            return null;
        }
    }
}
