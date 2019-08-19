using InDoOut_Core.Entities.Programs;

namespace InDoOut_Executable_Core.Storage
{
    /// <summary>
    /// Represents an interface that can store a <see cref="IProgram"/> to a storage device, and then
    /// read the data back off again to create the same <see cref="IProgram"/>.
    /// </summary>
    public interface IProgramStorer
    {
        /// <summary>
        /// Saves the <paramref name="program"/> to storage.
        /// </summary>
        /// <param name="program">The program to save.</param>
        /// <returns>Whether the program saved successfully.</returns>
        bool Save(IProgram program);

        /// <summary>
        /// Loads data into a program from storage.
        /// </summary>
        /// <param name="program">The program to load data into.</param>
        /// <returns>The stored program, or null if failed.</returns>
        bool Load(IProgram program);
    }
}
