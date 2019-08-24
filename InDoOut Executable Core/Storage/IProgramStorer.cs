using InDoOut_Core.Entities.Programs;

namespace InDoOut_Executable_Core.Storage
{
    public enum LoadResult
    {
        Unknown = -1,
        OK = 0,
        InvalidLocation,
        InvalidFile,
        InvalidExtension,
        InsufficientPermissions,
        MissingData,
        WrongVersion
    }

    public enum SaveResult
    {
        Unknown = -1,
        OK = 0,
        InvalidLocation,
        InsufficientPermissions,
        InvalidFileName
    }

    /// <summary>
    /// Represents an interface that can store a <see cref="IProgram"/> to a storage device, and then
    /// read the data back off again to create the same <see cref="IProgram"/>.
    /// </summary>
    public interface IProgramStorer
    {
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
        /// <returns>Whether the program saved successfully.</returns>
        SaveResult Save(IProgram program);

        /// <summary>
        /// Loads data into a program from storage.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <returns>The stored program, or null if failed.</returns>
        LoadResult Load(IProgram program);
    }
}
