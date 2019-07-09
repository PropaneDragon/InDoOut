namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A neutral output. These are used on neither good nor bad results.
    /// </summary>
    public class OutputNeutral : Output, IOutputNeutral
    {
        /// <summary>
        /// Creates a neutral output. Neutral outputs should be used where the result
        /// is neither good nor bad.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        public OutputNeutral(string name = "Output") : base(name)
        {
        }
    }
}
