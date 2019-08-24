using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Storage;
using InDoOut_Plugins.Loaders;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InDoOut Json Storage Tests")]
namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Stores a program in the JSON format.
    /// </summary>
    public class ProgramJsonStorer : IProgramStorer
    {
        /// <summary>
        /// The extension of the generated files.
        /// </summary>
        public string FileExtension => ".ido";

        /// <summary>
        /// The path to save to and load from.
        /// </summary>
        public string FilePath { get; set; } = null;

        IFunctionBuilder FunctionBuilder { get; set; } = null;

        ILoadedPlugins LoadedPlugins { get; set; } = null;

        /// <summary>
        /// Creates an instance of the JSON storer.
        /// </summary>
        /// <param name="path">The path to save to and load from.</param>
        /// <param name="builder">A function builder that can load functions with the program.</param>
        /// <param name="loadedPlugins">Available plugins that can be loaded.</param>
        public ProgramJsonStorer(IFunctionBuilder builder, ILoadedPlugins loadedPlugins, string path = null)
        {
            FilePath = path;
            FunctionBuilder = builder;
            LoadedPlugins = loadedPlugins;
        }

        /// <summary>
        /// Loads a program from the JSON storage file at the given <see cref="FilePath"/>.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <returns>The loaded program, or null if invalid.</returns>
        public LoadResult Load(IProgram program)
        {
            if (program != null)
            {
                try
                {
                    var jsonProgram = Load();
                    if (jsonProgram != null)
                    {
                        return jsonProgram.Set(program, FunctionBuilder, LoadedPlugins) ? LoadResult.OK : LoadResult.MissingData;
                    }
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is DirectoryNotFoundException)
                {
                    return LoadResult.InvalidLocation;
                }
                catch (IOException)
                {
                    return LoadResult.MissingData;
                }
                catch (UnauthorizedAccessException)
                {
                    return LoadResult.InsufficientPermissions;
                }
                catch
                {
                    return LoadResult.Unknown;
                }
            }

            return LoadResult.InvalidFile;
        }

        /// <summary>
        /// Saves a program to the given <see cref="FilePath"/>.
        /// </summary>
        /// <param name="program">The program to save.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        public SaveResult Save(IProgram program)
        {
            var jsonProgram = JsonProgram.CreateFromProgram(program);
            return Save(jsonProgram);
        }

        /// <summary>
        /// Saves a program to the given <see cref="FilePath"/> from JSON shell data.
        /// </summary>
        /// <param name="jsonProgram">The program to save.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        protected SaveResult Save(JsonProgram jsonProgram)
        {
            if (!string.IsNullOrEmpty(FilePath) && jsonProgram != null)
            {
                try
                {
                    var jsonText = JsonConvert.SerializeObject(jsonProgram, Formatting.Indented);

                    File.WriteAllText(FilePath, jsonText);

                    return File.Exists(FilePath) ? SaveResult.OK : SaveResult.InsufficientPermissions;
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is DirectoryNotFoundException)
                {
                    return SaveResult.InvalidLocation;
                }
                catch (UnauthorizedAccessException)
                {
                    return SaveResult.InsufficientPermissions;
                }
                catch
                {
                    return SaveResult.Unknown;
                }
            }

            return SaveResult.InvalidLocation;
        }

        /// <summary>
        /// Loads JSON shell data for a program from the given <see cref="FilePath"/>.
        /// </summary>
        /// <returns>A JSON shell program from the path, or null if failed.</returns>
        protected JsonProgram Load()
        {
            if (!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
            {
                var fileText = File.ReadAllText(FilePath);
                var jsonProgram = JsonConvert.DeserializeObject<JsonProgram>(fileText);

                return jsonProgram;
            }

            return null;
        }
    }
}
