namespace InDoOut_Core.Functions
{
    /// <summary>
    /// Represents a neutral output from a <see cref="IFunction"/>.
    /// Neutral outputs are neither positive or negative.
    /// See <see cref="IOutput"/> for more generalised info.
    /// </summary>
    /// <seealso cref="IOutput"/>
    interface IOutputNeutral : IOutput
    {
    }
}
