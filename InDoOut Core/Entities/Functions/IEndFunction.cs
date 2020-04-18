namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents a function that is to be run at the end
    /// of a program that returns a code.
    /// </summary>
    public interface IEndFunction : IFunction
    {
        /// <summary>
        /// The code that is returned when this function has run.
        /// </summary>
        string ReturnCode { get; }
    }
}
