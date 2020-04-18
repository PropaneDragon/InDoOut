using System;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A self contained block that can do processing independently of any other function. <see cref="EndFunction"/>s are
    /// made to be the final stage of a <see cref="Programs.IProgram"/>'s execution where they pass back a final value
    /// to the caller so further execution can continue, or to indicate some sort of error.
    /// </summary>
    public abstract class EndFunction : Function, IEndFunction
    {
        private readonly IProperty<string> _value;

        /// <summary>
        /// The return code to return back to the caller.
        /// </summary>
        public string ReturnCode { get; protected set; } = "0";

        /// <summary>
        /// The group this function belongs to.
        /// </summary>
        public override string Group => "End";

        /// <summary>
        /// Creates a basic <see cref="EndFunction"/>.
        /// </summary>
        public EndFunction()
        {
            _ = CreateInput("Return");

            _value = AddProperty(new Property<string>("Return value", "The value to return to the caller", true, "0"));
        }

        /// <summary>
        /// The function run when execution has started.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IInput"/> that caused this execution.</param>
        /// <returns>The <see cref="IOutput"/> to trigger as a result of this execution.</returns>
        protected override IOutput Started(IInput triggeredBy)
        {
            ReturnCode = _value?.FullValue ?? "0";

            return null;
        }
    }
}
