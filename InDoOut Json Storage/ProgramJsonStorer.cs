using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Storage;
using InDoOut_Plugins.Loaders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InDoOut Json Storage Tests")]
namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Stores a program in the JSON format.
    /// </summary>
    public class ProgramJsonStorer : ProgramStorer
    {
        /// <summary>
        /// The extension of the generated files.
        /// </summary>
        public override string FileExtension => ".ido";

        IFunctionBuilder FunctionBuilder { get; set; } = null;

        ILoadedPlugins LoadedPlugins { get; set; } = null;

        /// <summary>
        /// Creates an instance of the JSON storer.
        /// </summary>
        /// <param name="path">The path to save to and load from.</param>
        /// <param name="builder">A function builder that can load functions with the program.</param>
        /// <param name="loadedPlugins">Available plugins that can be loaded.</param>
        public ProgramJsonStorer(IFunctionBuilder builder, ILoadedPlugins loadedPlugins, string path = null) : base(path)
        {
            FunctionBuilder = builder;
            LoadedPlugins = loadedPlugins;
        }

        /// <summary>
        /// Loads a program from the JSON storage file at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <param name="path">The path to load from.</param>
        /// <returns>The loaded program, or null if invalid.</returns>
        protected override List<IFailureReport> TryLoad(IProgram program, string path)
        {
            var failures = new List<IFailureReport>();

            if (program != null)
            {
                try
                {
                    var jsonProgram = Load(path);
                    if (jsonProgram != null)
                    {
                        failures.AddRange(jsonProgram.Set(program, FunctionBuilder, LoadedPlugins));
                    }
                    else
                    {
                        failures.Add(new FailureReport((int)LoadResult.InvalidFile, $"The program could not be loaded from the given path ({path}).", true));
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
                    failures.Add(new FailureReport((int)LoadResult.Unknown, $"There was an unknown error while trying to deserialise the program to be loaded.", true));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)LoadResult.InvalidLocation, $"Invalid file location given ({path}).", true));
            }

            return failures;
        }

        /// <summary>
        /// Saves a program to the given <paramref name="path"/>.
        /// </summary>
        /// <param name="program">The program to save.</param>
        /// <param name="path">The path to save to.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        protected override List<IFailureReport> TrySave(IProgram program, string path)
        {
            var jsonProgram = JsonProgram.CreateFromProgram(program);
            return Save(jsonProgram, path);
        }

        /// <summary>
        /// Saves a program to the given <paramref name="path"/> from JSON shell data.
        /// </summary>
        /// <param name="jsonProgram">The program to save.</param>
        /// <param name="path">The path to save to.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        protected List<IFailureReport> Save(JsonProgram jsonProgram, string path)
        {
            var failures = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(path) && jsonProgram != null)
            {
                try
                {
                    var jsonText = JsonConvert.SerializeObject(jsonProgram, Formatting.Indented);

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
                    failures.Add(new FailureReport((int)SaveResult.Unknown, $"There was an unknown error while trying to serialise the program to be saved.", true));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)SaveResult.InvalidLocation, $"Invalid file location given ({path})", true));
            }

            return failures;
        }

        /// <summary>
        /// Loads JSON shell data for a program from the given <paramref name="path"/>.
        /// </summary>
        /// <returns>A JSON shell program from the path, or null if failed.</returns>
        protected JsonProgram Load(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var fileText = File.ReadAllText(path);
                var jsonProgram = JsonConvert.DeserializeObject<JsonProgram>(fileText);

                return jsonProgram;
            }

            return null;
        }
    }
}
