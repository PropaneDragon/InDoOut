namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A self contained block that can do processing independently of any other function. <see cref="StartFunction"/>s are
    /// slightly different to <see cref="Function"/>s, where instead of only being able to be triggered by a given <see cref="IInput"/>,
    /// they are triggered automatically at the start of a <see cref="Programs.IProgram"/>.
    /// </summary>
    public abstract class StartFunction : Function, IStartFunction
    {
        /// <summary>
        /// The 'started' output that should be triggered when this function is called.
        /// </summary>
        protected IOutput OutputStart { get; private set; } = null;

        /// <summary>
        /// The group this function belongs to.
        /// </summary>
        public override string Group => "Start";

        /// <summary>
        /// Creates a basic start function.
        /// </summary>
        public StartFunction()
        {
            OutputStart = CreateOutput(OutputType.Neutral, "Program started");
        }
    }
}
