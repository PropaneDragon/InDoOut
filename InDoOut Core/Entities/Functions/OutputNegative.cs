namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A negative output. These are generally used on bad results.
    /// </summary>
    public class OutputNegative : Output, IOutputNegative
    {
        /// <summary>
        /// Creates a negative output. Negative outputs should be used where the result
        /// is bad.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        public OutputNegative(string name = "Output") : base(name)
        {
        }
    }
}
