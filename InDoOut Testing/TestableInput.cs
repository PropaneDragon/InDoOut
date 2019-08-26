using InDoOut_Core.Entities.Functions;

namespace InDoOut_Testing
{
    /// <summary>
    /// A generic input that can track how many times it was activated and the last
    /// entity that triggered it.
    /// </summary>
    public class TestableInput : Input
    {
        /// <summary>
        /// Whether this connection was triggered.
        /// </summary>
        public bool Triggered { get; private set; } = false;

        /// <summary>
        /// The number of times this connection has been triggered.
        /// </summary>
        public int TriggeredCount { get; private set; } = 0;

        /// <summary>
        /// The last entity that activated this connection.
        /// </summary>
        public IOutput LastTriggeredBy { get; private set; } = null;

        /// <summary>
        /// Creates a new testable input
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public TestableInput(IFunction parent = null, string name = "Input") : base(parent, name)
        {
        }

        /// <summary>
        /// Resets the triggered count and triggered state back to the original
        /// values.
        /// </summary>
        /// <param name="resetCount"></param>
        public void Reset(bool resetCount = true)
        {
            LastTriggeredBy = null;
            Triggered = false;

            if (resetCount)
            {
                TriggeredCount = 0;
            }
        }

        /// <summary>
        /// Processes the generic connection and increases the trigger count.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this connection.</param>
        protected override void Process(IOutput triggeredBy)
        {
            LastTriggeredBy = triggeredBy;
            Triggered = true;
            ++TriggeredCount;

            base.Process(triggeredBy);
        }
    }
}
