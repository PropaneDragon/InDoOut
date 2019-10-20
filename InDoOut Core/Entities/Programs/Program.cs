using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Logging;
using InDoOut_Core.Threading.Safety;
using InDoOut_Core.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Core.Entities.Programs
{
    /// <summary>
    /// A collection of <see cref="IFunction"/> entities linked together and started through the
    /// <see cref="StartFunctions"/>.
    /// </summary>
    public class Program : Entity, IProgram
    {
        private readonly object _lastTriggerTimeLock = new object();

        private DateTime _lastTriggerTime = DateTime.MinValue;

        /// <summary>
        /// All functions within this program.
        /// </summary>
        public List<IFunction> Functions { get; protected set; } = new List<IFunction>();

        /// <summary>
        /// All <see cref="IStartFunction"/>s within this program.
        /// These are triggered when the program is triggered, and the program will not
        /// start without at least one.
        /// </summary>
        public List<IStartFunction> StartFunctions => Functions.Where(function => typeof(IStartFunction).IsAssignableFrom(function.GetType())).Cast<IStartFunction>().ToList();

        /// <summary>
        /// Values to pass into <see cref="StartFunctions"/> when the program is started.
        /// </summary>
        public List<string> PassthroughValues { get; private set; } = new List<string>();

        /// <summary>
        /// Whether any of the functions within this program are running.
        /// </summary>
        public bool Running => Functions.Any(function => function.Running || function.Outputs.Any(output => output != function && output.Running));

        /// <summary>
        /// Whether any of the functions within this program are still stopping.
        /// </summary>
        public bool Stopping => Functions.Any(function => function.State == State.Stopping && function.Running);

        /// <summary>
        /// The name of this program.
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// The last time this program was triggered.
        /// </summary>
        public DateTime LastTriggerTime { get { lock (_lastTriggerTimeLock) return _lastTriggerTime; } }

        /// <summary>
        /// The current variable store for all program variables.
        /// </summary>
        /// <seealso cref="VariableStore"/>
        public IVariableStore VariableStore { get; protected set; } = new VariableStore();

        /// <summary>
        /// Creates a program with optional passthrough values.
        /// </summary>
        /// <param name="passthroughValues">Values to pass into the <see cref="IStartFunction"/>s when triggered.</param>
        /// <seealso cref="Trigger(IEntity)"/>
        public Program(params string[] passthroughValues)
        {
            PassthroughValues = passthroughValues.ToList();

            Log.Instance.Header($"Program created: {this}");
        }

        /// <summary>
        /// Add a function to the program.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <returns>Whether the function was added.</returns>
        public bool AddFunction(IFunction function)
        {
            Log.Instance.Info($"Attempting to add ", function, $" to: {this}");

            if (function != null && !Functions.Contains(function))
            {
                function.VariableStore = VariableStore;

                Functions.Add(function);

                Log.Instance.Info("Added ", function, $" to {this}");

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a function from the program.
        /// </summary>
        /// <param name="function">The function to remove.</param>
        /// <returns>Whether the function was found and removed.</returns>
        public bool RemoveFunction(IFunction function)
        {
            if (function != null && Functions.Contains(function))
            {
                _ = Functions.Remove(function);

                Log.Instance.Info("Removed ", function, $" from {this}");

                return true;
            }

            return false;
        }

        /// <summary>
        /// Whether this program can be triggered by the given <see cref="IEntity"/>.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether this program can be triggered by the entity.</returns>
        public bool CanBeTriggered(IEntity entity)
        {
            return false;
        }

        /// <summary>
        /// Trigger this program. This will start all available <see cref="StartFunctions"/>.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IEntity"/> that triggered this.</param>
        public void Trigger(IEntity triggeredBy)
        {
            Log.Instance.Header($"Triggered {this}");

            lock (_lastTriggerTimeLock)
            {
                _lastTriggerTime = DateTime.Now;
            }

            foreach (var startFunction in StartFunctions)
            {
                for (var index = 0; index < PassthroughValues.Count; ++index)
                {
                    if (index < startFunction.PassthroughResults.Count)
                    {
                        startFunction.PassthroughResults[index].RawValue = PassthroughValues[index];
                    }
                }

                startFunction.Trigger(null);
            }
        }

        /// <summary>
        /// Attempts to stop program execution by sending the <see cref="IFunction.PolitelyStop"/> call
        /// to every function in the program. It's up to the function to stop cleanly, so it might take some
        /// time to fully stop. See <see cref="Stopping"/> to see the state of this procedure.
        /// </summary>
        public void Stop()
        {
            Log.Instance.Header($"Stopping {this}");

            foreach (var function in Functions)
            {
                 _ = TryGet.ExecuteOrFail(() => function?.PolitelyStop());
            }
        }

        /// <summary>
        /// Sets the program name to the given value.
        /// </summary>
        /// <param name="name">The program name to set.</param>
        public void SetName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Checks whether the program has been triggered since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the program has been triggered since the given time.</returns>
        public bool HasBeenTriggeredSince(DateTime time)
        {
            return LastTriggerTime >= time;
        }

        /// <summary>
        /// Checks whether the program has been triggered within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the program has been triggered within the last 5 seconds.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the program has been triggered within the given time.</returns>
        public bool HasBeenTriggeredWithin(TimeSpan time)
        {
            return LastTriggerTime >= DateTime.Now - time;
        }

        /// <summary>
        /// Returns a string representation of a program.
        /// </summary>
        /// <returns>A string representation of the program.</returns>
        public override string ToString()
        {
            return $"[PROGRAM {base.ToString()} [Name: {Name}] [Running: {Running}]]";
        }
    }
}
