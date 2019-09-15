using InDoOut_Core.Entities.Core;
using InDoOut_Core.Threading.Safety;
using InDoOut_Core.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A self contained block that can do processing independently of any other function. These have inputs and
    /// outputs, where the inputs trigger the function which then trigger an output.
    /// </summary>
    public abstract class Function : InteractiveEntity<IOutput, IInput>, IFunction
    {
        private readonly object _stateLock = new object();
        private readonly object _inputsLock = new object();
        private readonly object _propertiesLock = new object();
        private readonly object _resultsLock = new object();

        private State _state = State.Unknown;
        private List<IInput> _inputs = new List<IInput>();
        private List<IProperty> _properties = new List<IProperty>();
        private List<IResult> _results = new List<IResult>();
        private Dictionary<IProperty, IResult> _mirroredResults = new Dictionary<IProperty, IResult>();

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
        /// The current variable store.
        /// </summary>
        public IVariableStore VariableStore { get; set; } = null;

        /// <summary>
        /// The inputs that belong to this function.
        /// </summary>
        public List<IInput> Inputs
        {
            get { lock (_inputsLock) return _inputs; }
            private set { lock (_inputsLock) _inputs = value; }
        }

        /// <summary>
        /// The properties that this function accepts.
        /// </summary>
        public List<IProperty> Properties
        {
            get { lock (_propertiesLock) return _properties; }
            protected set { lock (_propertiesLock) _properties = value; }
        }

        /// <summary>
        /// The results that this function gives.
        /// </summary>
        public List<IResult> Results
        {
            get { lock (_resultsLock) return _results; }
            protected set { lock (_resultsLock) _results = value; }
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
            return output != null && AddConnection(output) ? output : null;
        }

        /// <summary>
        /// Creates an output for this function.
        /// </summary>
        /// <param name="name">The name of the output.</param>
        /// <param name="outputType">The type of output to create. Different types return different classes.</param>
        /// <returns>The output that was created.</returns>
        protected IOutput CreateOutput(OutputType outputType, string name = "Output") => CreateOutput(name, outputType);

        /// <summary>
        /// Adds a property to the function and returns the same property as a result. This allows it to be created and added
        /// on the same line.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IProperty"/> to add.</typeparam>
        /// <param name="property">The property to add.</param>
        /// <param name="mirrorAsResult">Whether to mirror this property as a result automatically. This will copy the property value into the result value when activated.</param>
        /// <returns>The given <paramref name="property"/>.</returns>
        protected T AddProperty<T>(T property, bool mirrorAsResult = true) where T : IProperty
        {
            if (property != null && !Properties.Contains(property))
            {
                Properties.Add(property);

                _ = property.Connect(this);

                if (mirrorAsResult)
                {
                    _mirroredResults[property] = AddResult(new Result($"{property.Name} *", $"Passthrough: {property.Description}", property.RawComputedValue));
                }
            }

            return property;
        }

        /// <summary>
        /// Adds a result to the function and returns the same result as a result. This allows it to be created and added
        /// on the same line.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IResult"/> to add.</typeparam>
        /// <param name="result">The property to add.</param>
        /// <returns>The given <paramref name="result"/>.</returns>
        protected T AddResult<T>(T result) where T : IResult
        {
            if (result != null && !Results.Contains(result))
            {
                Results.Add(result);
            }

            return result;
        }

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
            return outputType switch
            {
                OutputType.Positive => new OutputPositive(name),
                OutputType.Negative => new OutputNegative(name),
                OutputType.Neutral => new OutputNeutral(name),

                _ => (IOutput)null,
            };
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
                    foreach (var mirroredResult in _mirroredResults)
                    {
                        if (mirroredResult.Value != null && mirroredResult.Key != null)
                        {
                            mirroredResult.Value.RawValue = mirroredResult.Key.RawComputedValue;
                        }
                    }

                    var nextOutput = Started(triggeredBy);

                    foreach (var result in Results)
                    {
                        if (result.CanBeTriggered(this))
                        {
                            result.Trigger(this);
                        }
                    }

                    while (Results.Any(result => result.Running))
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1));
                    }

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
