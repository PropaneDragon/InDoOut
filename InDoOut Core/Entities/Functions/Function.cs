using InDoOut_Core.Entities.Core;
using System;
using System.Collections.Generic;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A self contained block that can do processing independently of any other function. These have inputs and
    /// outputs, where the inputs trigger the function which then trigger an output.
    /// </summary>
    public abstract class Function : InteractiveEntity<IOutput, IInput>, IFunction
    {
        /// <summary>
        /// The current function state. See <see cref="State"/> for more
        /// info on the states.
        /// </summary>
        /// <seealso cref="State"/>
        public State State { get; private set; } = State.Unknown;

        /// <summary>
        /// The inputs that belong to this function.
        /// </summary>
        public List<IInput> Inputs { get; private set; } = new List<IInput>();

        /// <summary>
        /// The outputs that belong to this function.
        /// </summary>
        public List<IOutput> Outputs => Connections;

        /// <summary>
        /// The function name.
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// Creates a basic function.
        /// </summary>
        public Function(string name = null)
        {
            Name = name;
            State = State.Waiting;
        }

        /// <summary>
        /// Creates an input for this function.
        /// </summary>
        /// <param name="name">The name of the input.</param>
        /// <returns>The input that was created.</returns>
        protected IInput CreateInput(string name = "Input")
        {
            var input = new Input(this, name);

            if (!Inputs.Contains(input))
            {
                Inputs.Add(input);

                return input;
            }

            return null;
        }

        /// <summary>
        /// Creates an output for this function.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        /// <returns>The output that was created.</returns>
        protected IOutput CreateOutput(string name = "Output")
        {
            var output = new Output(name);

            if(AddConnection(output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Start processing the function code. Sets up all states.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this.</param>
        protected override void Process(IInput triggeredBy)
        {
            if (State != State.Disabled)
            {
                State = State.Processing;

                try
                {
                    var nextOutput = Started(triggeredBy);
                    if (nextOutput != null && Outputs.Contains(nextOutput) && nextOutput.CanBeTriggered(this))
                    {
                        nextOutput.Trigger(this);
                    }

                    State = State.Waiting;
                }
                catch (Exception ex)
                {
                    State = State.InError;

                    var trace = ex.StackTrace;

                    //Todo: Log this.
                }
            }
        }

        /// <summary>
        /// Start the main code, given a <see cref="IInput"/>. This will return the <see cref="IOutput"/>
        /// that should be triggered at the end of processing.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this.</param>
        /// <returns>An <see cref="IOutput"/> that should be triggered after this code.</returns>
        protected abstract IOutput Started(IInput triggeredBy);
    }
}
