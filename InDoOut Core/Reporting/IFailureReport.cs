namespace InDoOut_Core.Reporting
{
    /// <summary>
    /// Represents a failure report that contains details on a failure
    /// that has occurred.
    /// </summary>
    public interface IFailureReport
    {
        /// <summary>
        /// Whether this failure is a critical failure and could cause issues if
        /// continued. If false, this is a recoverable state and can be continued.
        /// </summary>
        public bool Critical { get; }

        /// <summary>
        /// The ID of the failure.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// A summary of the failure.
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// Detailed information (if available) about the failure that has
        /// occurred.
        /// </summary>
        public string DetailedReport { get; }
    }
}