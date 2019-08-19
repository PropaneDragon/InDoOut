using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Storage;
using InDoOut_Plugins.Loaders;
using Newtonsoft.Json;
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
        /// The path to save to and load from.
        /// </summary>
        public string Path { get; private set; } = null;

        IFunctionBuilder FunctionBuilder { get; set; } = null;

        ILoadedPlugins LoadedPlugins { get; set; } = null;

        /// <summary>
        /// Creates an instance of the JSON storer.
        /// </summary>
        /// <param name="path">The path to save to and load from.</param>
        /// <param name="builder">A function builder that can load functions with the program.</param>
        /// <param name="loadedPlugins">Available plugins that can be loaded.</param>
        public ProgramJsonStorer(string path, IFunctionBuilder builder, ILoadedPlugins loadedPlugins)
        {
            Path = path;
            FunctionBuilder = builder;
            LoadedPlugins = loadedPlugins;
        }

        /// <summary>
        /// Loads a program from the JSON storage file at the given <see cref="Path"/>.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <returns>The loaded program, or null if invalid.</returns>
        public bool Load(IProgram program)
        {
            if (program != null)
            {
                var jsonProgram = Load();
                if (jsonProgram != null)
                {
                    return jsonProgram.Set(program, FunctionBuilder, LoadedPlugins);
                }
            }

            return false;
        }

        /// <summary>
        /// Saves a program to the given <see cref="Path"/>.
        /// </summary>
        /// <param name="program">The program to save.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        public bool Save(IProgram program)
        {
            var jsonProgram = JsonProgram.CreateFromProgram(program);
            return Save(jsonProgram);
        }

        /// <summary>
        /// Saves a program to the given <see cref="Path"/> from JSON shell data.
        /// </summary>
        /// <param name="jsonProgram">The program to save.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        protected bool Save(JsonProgram jsonProgram)
        {
            if (!string.IsNullOrEmpty(Path) && jsonProgram != null)
            {
                try
                {
                    var jsonText = JsonConvert.SerializeObject(jsonProgram, Formatting.Indented);

                    File.WriteAllText(Path, jsonText);

                    return File.Exists(Path);
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// Loads JSON shell data for a program from the given <see cref="Path"/>.
        /// </summary>
        /// <returns>A JSON shell program from the path, or null if failed.</returns>
        protected JsonProgram Load()
        {
            if (!string.IsNullOrEmpty(Path) && File.Exists(Path))
            {
                try
                {
                    var fileText = File.ReadAllText(Path);
                    var jsonProgram = JsonConvert.DeserializeObject<JsonProgram>(fileText);

                    return jsonProgram;
                }
                catch { }
            }

            return null;
        }
    }
}
