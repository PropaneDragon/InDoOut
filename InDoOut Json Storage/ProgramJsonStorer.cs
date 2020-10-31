using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Localisation;
using InDoOut_Executable_Core.Storage;
using InDoOut_Plugins.Loaders;
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
        /// The name of the file extension that will be readable by the user.
        /// </summary>
        public override string FileReadableName => $"{Branding.Instance.AppNameShort} Program";

        /// <summary>
        /// The extension of the generated files.
        /// </summary>
        public override string FileExtension => ".ido";

        IFunctionBuilder FunctionBuilder { get; set; } = null;

        ILoadedPlugins LoadedPlugins { get; set; } = null;

        /// <summary>
        /// Creates an instance of the JSON storer.
        /// </summary>
        /// <param name="builder">A function builder that can load functions with the program.</param>
        /// <param name="loadedPlugins">Available plugins that can be loaded.</param>
        public ProgramJsonStorer(IFunctionBuilder builder, ILoadedPlugins loadedPlugins) : base()
        {
            FunctionBuilder = builder;
            LoadedPlugins = loadedPlugins;
        }

        /// <summary>
        /// Loads a program from the JSON storage file at the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <param name="stream">The path to load from.</param>
        /// <returns>The loaded program, or null if invalid.</returns>
        protected override List<IFailureReport> TryLoad(IProgram program, Stream stream)
        {
            var failures = new List<IFailureReport>();

            if (program != null)
            {
                try
                {
                    var jsonProgram = GenericJsonStorer.Load<JsonProgram>(stream);
                    if (jsonProgram != null)
                    {
                        failures.AddRange(jsonProgram.Set(program, FunctionBuilder, LoadedPlugins));
                    }
                    else
                    {
                        failures.Add(new FailureReport((int)LoadResult.InvalidFile, $"The program contents could not be loaded.", true));
                    }
                }
                catch (IOException)
                {
                    failures.Add(new FailureReport((int)LoadResult.InvalidFile, $"The data couldn't be loaded.", true));
                }
                catch (UnauthorizedAccessException)
                {
                    failures.Add(new FailureReport((int)LoadResult.InsufficientPermissions, $"Access couldn't be given to load the program.", true));
                }
                catch
                {
                    failures.Add(new FailureReport((int)LoadResult.Unknown, $"There was an unknown error while trying to deserialise the program to be loaded.", true));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)LoadResult.InvalidLocation, $"There was no program present to load into.", true));
            }

            return failures;
        }

        /// <summary>
        /// Saves a program to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="program">The program to save.</param>
        /// <param name="stream">The path to save to.</param>
        /// <returns>Whether or not the program could be saved to the path.</returns>
        protected override List<IFailureReport> TrySave(IProgram program, Stream stream)
        {
            var jsonProgram = JsonProgram.CreateFromProgram(program);
            return GenericJsonStorer.Save(jsonProgram, stream);
        }
    }
}
