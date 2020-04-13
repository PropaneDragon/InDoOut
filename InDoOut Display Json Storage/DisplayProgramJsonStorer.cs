using InDoOut_Core.Functions;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Display_Json_Storage
{
    /// <summary>
    /// An implementation of the <see cref="ProgramJsonStorer"/> but has overrides to store it as
    /// the IDO Display file type.
    /// </summary>
    public class DisplayProgramJsonStorer : ProgramJsonStorer
    {
        /// <summary>
        /// The name of the file extension that will be readable by the user.
        /// </summary>
        public override string FileReadableName => "ido Display interface";

        /// <summary>
        /// The extension of the generated files.
        /// </summary>
        public override string FileExtension => ".idod";

        /// <summary>
        /// Creates an instance of the JSON storer.
        /// </summary>
        /// <param name="path">The path to save to and load from.</param>
        /// <param name="builder">A function builder that can load functions with the program.</param>
        /// <param name="loadedPlugins">Available plugins that can be loaded.</param>
        public DisplayProgramJsonStorer(IFunctionBuilder builder, ILoadedPlugins loadedPlugins, string path = null) : base(builder, loadedPlugins, path)
        {
        }
    }
}
