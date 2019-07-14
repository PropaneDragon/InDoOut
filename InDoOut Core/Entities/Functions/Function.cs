using InDoOut_Core.Entities.Core;
using InDoOut_Core.Threading.Safety;
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
        private object _stateLock = new object();
        private object _inputsLock = new object();

        private State _state = State.Unknown;
        private List<IInput> _inputs = new List<IInput>();

        /// <summary>
        /// Stop has been requested on the task, and it should be terminated as soon
        /// as possible.
        /// </summary>
        public bool StopRequested { get; private set; } = false;

        /// <summary>
        /// The current function state. See <see cref="State"/> for more
        /// info on the states.
        /// </summary>
        /// <seealso cref="State"/>
        public State State
        {
            get { lock (_stateLock) return _state; }
            private set { lock (_stateLock) _state = value; }
        }

        /// <summary>
        /// The inputs that belong to this function.
        /// </summary>
        public List<IInput> Inputs
        {
            get { lock (_inputsLock) return _inputs; }
            private set { lock (_inputsLock) _inputs = value; }
        }

        /// <summary>
        /// The outputs that belong to this function.
        /// </summary>
        public List<IOutput> Outputs => Connections;

        /// <summary>
        /// An exception safe version of <see cref="Name"/>
        /// </summary>
        public string SafeName => TryGet.ValueOrDefault(() => Name);

        /// <summary>
        /// An exception safe version of <see cref="Description"/>
        /// </summary>
        public string SafeDescription => TryGet.ValueOrDefault(() => Description);

        /// <summary>
        /// An exception safe version of <see cref="Group"/>
        /// </summary>
        public string SafeGroup => TryGet.ValueOrDefault(() => Group);

        /// <summary>
        /// An exception safe version of <see cref="Keywords"/>
        /// </summary>
        public string[] SafeKeywords => TryGet.ValueOrDefault(() => Keywords);

        /// <summary>
        /// The description of what the function does.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The name of the function.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The group this function belongs to. This is to allow for easier categorisation
        /// of functions.
        /// </summary>
        public abstract string Group { get; }

        /// <summary>
        /// Keywords associated with this function. This allows for similar words to match
        /// this function when being searched for.
        /// </summary>
        public abstract string[] Keywords { get; }

        /// <summary>
        /// Creates a basic function.
        /// </summary>
        public Function()
        {
            State = State.Waiting;
        }

        /// <summary>
        /// Makes a request for the function to stop when it's safe to do so, for example
        /// on filesystem actions. If there's nothing in place, the underlying code doesn't
        /// have to listen to this request, and provisions may not be in place to stop it.
        /// For any code that listens for <see cref="StopRequested"/> they will stop when
        /// this method is called.
        /// </summary>
        /// <seealso cref="StopRequested"/>
        public void PolitelyStop()
        {
            StopRequested = true;
            State = State.Stopping;
        }

        /// <summary>
        /// Creates an input for this function.
        /// </summary>
        /// <param name="name">The name of the input.</param>
        /// <returns>The input that was created.</returns>
        protected IInput CreateInput(string name = "Input")
        {
            var input = TryGet.ValueOrDefault(() => BuildInput(name), null);
            if (input != null && !Inputs.Contains(input))
            {
                Inputs.Add(input);

                return input;
            }

            return null;
        }

        /// <summary>
        /// Creates an output for this function.
        /// </summary>
        /// <param name="outputType">The type of output to create. Different types return different classes.</param>
        /// <param name="name">The name of the output.</param>
        /// <returns>The output that was created.</returns>
        protected IOutput CreateOutput(string name = "Output", OutputType outputType = OutputType.Neutral)
        {
            var output = TryGet.ValueOrDefault(() => BuildOutput(name, outputType), null);
            if (output != null && AddConnection(output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Creates an output for this function.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        /// <param name="outputType">The type of output to create. Different types return different classes.</param>
        /// <returns>The output that was created.</returns>
        protected IOutput CreateOutput(OutputType outputType, string name = "Output") => CreateOutput(name, outputType);

        /// <summary>
        /// A builder for creating an <see cref="IInput"/> entity
        /// when requested.
        /// </summary>
        /// <param name="name">The name of the input.</param>
        /// <returns>A new input for a given name.</returns>
        protected virtual IInput BuildInput(string name)
        {
            return new Input(this, name);
        }

        /// <summary>
        /// A builder for creating an <see cref="IOutput"/> entity
        /// when requested.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        /// <param name="outputType">The type of output to create.</param>
        /// <returns>A new output for a given name.</returns>
        protected virtual IOutput BuildOutput(string name, OutputType outputType)
        {
            switch (outputType)
            {
                case OutputType.Positive:
                    return new OutputPositive(name);
                case OutputType.Negative:
                    return new OutputNegative(name);
                case OutputType.Neutral:
                    return new OutputNeutral(name);
            }

            return null;
        }

        /// <summary>
        /// Start processing the function code. Sets up all states.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this.</param>
        protected override void Process(IInput triggeredBy)
        {
            StopRequested = false;

            if (State != State.Disabled)
            {
                State = State.Processing;

                try
                {
                    var nextOutput = Started(triggeredBy);
                    if (State != State.Stopping && nextOutput != null && Outputs.Contains(nextOutput) && nextOutput.CanBeTriggered(this))
                    {
                        nextOutput.Trigger(this);
                    }

                    State = State.Waiting;
                }
                catch (Exception)
                {
                    State = State.InError;
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
