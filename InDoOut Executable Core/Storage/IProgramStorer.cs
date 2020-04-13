using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Reporting;
using System.Collections.Generic;

namespace InDoOut_Executable_Core.Storage
{
    /// <summary>
    /// Represents an interface that can store a <see cref="IProgram"/> to a storage device, and then
    /// read the data back off again to create the same <see cref="IProgram"/>.
    /// </summary>
    public interface IProgramStorer
    {
        /// <summary>
        /// The name of the file extension that will be readable by the user.
        /// </summary>
        string FileReadableName { get; }

        /// <summary>
        /// The extension of the files that are generated or loaded.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// The path of the file to load or save.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Saves the <paramref name="program"/> to storage.
        /// </summary>
        /// <param name="program">The program to save.</param>
        /// <returns>Failure reports if saving has failed, or an empty list if succeeded.</returns>
        List<IFailureReport> Save(IProgram program);

        /// <summary>
        /// Loads data into a program from storage.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <returns>Failure reports if loading has failed, or an empty list if succeeded.</returns>
        List<IFailureReport> Load(IProgram program);
    }
}
