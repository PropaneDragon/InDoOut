namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A positive output. These are generally used on good results.
    /// </summary>
    public class OutputPositive : Output, IOutputPositive
    {
        /// <summary>
        /// Creates a positive output. Positive outputs should be used where the result
        /// is good.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        public OutputPositive(string name = "Output") : base(name)
        {
        }
    }
}
