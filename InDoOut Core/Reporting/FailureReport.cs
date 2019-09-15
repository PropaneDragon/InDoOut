namespace InDoOut_Core.Reporting
{
    /// <summary>
    /// A failure report containing details about a failure that
    /// has occurred.
    /// </summary>
    public class FailureReport : IFailureReport
    {
        /// <summary>
        /// Whether this failure is a critical failure and could cause issues if
        /// continued. If false, this is a recoverable state and can be continued.
        /// </summary>
        public bool Critical { get; private set; } = false;

        /// <summary>
        /// The ID of the failure.
        /// </summary>
        public int Id { get; private set; } = 0;

        /// <summary>
        /// A summary of the failure.
        /// </summary>
        public string Summary { get; private set; } = null;

        /// <summary>
        /// Detailed information (if available) aobut the failure that
        /// has occurred.
        /// </summary>
        public string DetailedReport { get; private set; } = null;

        /// <summary>
        /// Constructs a basic failure report.
        /// </summary>
        /// <param name="id">The ID of the failure.</param>
        /// <param name="summary">A summary of the failure that has occurred.</param>
        /// <param name="detailedReport">An optional detailed report giving more information about the failure.</param>
        /// <param name="critical">Whether or not this failure is critical, and continuing after this could cause problems.</param>
        public FailureReport(int id, string summary, string detailedReport = null, bool critical = false)
        {
            Critical = critical;
            Id = id;
            Summary = summary;
            DetailedReport = detailedReport;
        }

        /// <summary>
        /// Constructs a basic failure report.
        /// </summary>
        /// <param name="id">The ID of the failure.</param>
        /// <param name="summary">A summary of the failure that has occurred.</param>
        /// <param name="critical">Whether or not this failure is critical, and continuing after this could cause problems.</param>
        public FailureReport(int id, string summary, bool critical) : this(id, summary, null, critical)
        {
        }
    }
}
