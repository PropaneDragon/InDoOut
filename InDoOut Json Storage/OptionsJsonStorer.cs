using InDoOut_Core.Options;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Stores a set of options in the JSON format.
    /// </summary>
    public class OptionsJsonStorer : OptionsStorer
    {
        /// <summary>
        /// Creates a basic <see cref="OptionsJsonStorer"/> without a path, which can be set later using
        /// <see cref="OptionsStorer.FilePath"/>.
        /// </summary>
        public OptionsJsonStorer() : base()
        {
        }

        /// <summary>
        /// Creates a <see cref="OptionsJsonStorer"/> with a <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to the file that will be saved or loaded.</param>
        public OptionsJsonStorer(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// The extension of the generated files.
        /// </summary>
        public override string FileExtension => ".idosettings";

        /// <summary>
        /// Attempts to load option values into the given list of <paramref name="optionHolder"/> from the given
        /// <paramref name="path"/>.
        /// </summary>
        /// <param name="optionHolder">The options to load values into.</param>
        /// <param name="path">The location to load options from.</param>
        /// <returns></returns>
        protected override List<IFailureReport> TryLoad(IOptionHolder optionHolder, string path)
        {
            var failures = new List<IFailureReport>();

            if (optionHolder != null)
            {
                try
                {
                    var jsonOptions = GenericJsonStorer.Load<JsonOptions>(path);
                    if (jsonOptions != null)
                    {
                        failures.AddRange(jsonOptions.SetListValues(optionHolder));
                    }
                    else if (File.Exists(path))
                    {
                        failures.Add(new FailureReport((int)LoadResult.InvalidFile, $"The options could not be loaded from the given path ({path}).", true));
                    }
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is DirectoryNotFoundException)
                {
                    failures.Add(new FailureReport((int)LoadResult.InvalidLocation, $"The location given is invalid ({path}).", true));
                }
                catch (IOException)
                {
                    failures.Add(new FailureReport((int)LoadResult.InvalidFile, $"The given file doesn't appear to be valid ({path}).", true));
                }
                catch (UnauthorizedAccessException)
                {
                    failures.Add(new FailureReport((int)LoadResult.InsufficientPermissions, $"You don't have access to the file path given ({path}).", true));
                }
                catch
                {
                    failures.Add(new FailureReport((int)LoadResult.Unknown, $"There was an unknown error while trying to deserialise the options to be loaded.", true));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)LoadResult.InvalidLocation, $"Invalid file location given ({path}).", true));
            }

            return failures;
        }

        /// <summary>
        /// Attempts to save the given list of <paramref name="optionHolder"/> to the given <paramref name="path"/>
        /// on disk.
        /// </summary>
        /// <param name="optionHolder">The options to save.</param>
        /// <param name="path">The path to save the options to.</param>
        /// <returns></returns>
        protected override List<IFailureReport> TrySave(IOptionHolder optionHolder, string path)
        {
            var jsonProgram = JsonOptions.CreateFromList(optionHolder);
            return GenericJsonStorer.Save(jsonProgram, path);
        }
    }
}
