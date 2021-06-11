using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Logging;
using InDoOut_Core.Threading.Safety;
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
        private readonly object _lastCompletionTimeLock = new object();

        private readonly DateTime _lastCompletionTime = DateTime.MinValue;

        private string _name = null;
        private DateTime _lastTriggerTime = DateTime.MinValue;

        /// <summary>
        /// Whether any of the functions within this program are running.
        /// </summary>
        public bool Running => Functions.Any(function => function.Running || function.Outputs.Any(output => output != function && output.Running));

        /// <summary>
        /// Whether the program is currently tidying up before completion.
        /// </summary>
        public bool Finishing => false;

        /// <summary>
        /// Whether any of the functions within this program are still stopping.
        /// </summary>
        public bool Stopping => Functions.Any(function => function.State == State.Stopping && function.Running);

        /// <summary>
        /// The return code of this program after execution. If there are any
        /// <see cref="IEndFunction"/>s present as part of this program and they have
        /// executed with a value in the <see cref="IEndFunction.ReturnCode"/> then that
        /// will be represented in the return code of this program.
        /// <para/>
        /// Note: If multiple <see cref="IEndFunction"/>s have been triggered as part of program
        /// execution, the first one to be found will be returned. By default this return code will be "0".
        /// </summary>
        public string ReturnCode => EndFunctions?.FirstOrDefault(function => function.HasCompletedSince(LastTriggerTime))?.ReturnCode ?? "0";

        /// <summary>
        /// The last time this program was triggered.
        /// </summary>
        public DateTime LastTriggerTime { get { lock (_lastTriggerTimeLock) { return _lastTriggerTime; } } }

        /// <summary>
        /// The time this program last completed a run (successfully or unsuccessfully).
        /// </summary>
        public DateTime LastCompletionTime { get { lock (_lastCompletionTimeLock) { return _lastCompletionTime; } } }

        /// <summary>
        /// Values to pass into <see cref="StartFunctions"/> when the program is started.
        /// </summary>
        public List<string> PassthroughValues { get; private set; } = new List<string>();

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
        /// All <see cref="IEndFunction"/>s within this program.
        /// If one of these are called then this means the program has potentially ended, and the
        /// resulting program <see cref="ReturnCode"/> will reflect the <see cref="IEndFunction"/> that was called.
        /// </summary>
        public List<IEndFunction> EndFunctions => Functions.Where(function => typeof(IEndFunction).IsAssignableFrom(function.GetType())).Cast<IEndFunction>().ToList();

        /// <summary>
        /// The name of this program.
        /// </summary>
        public virtual string Name { get => _name ?? "Untitled"; protected set => _name = value; }

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
        public virtual bool AddFunction(IFunction function)
        {
            Log.Instance.Info($"Attempting to add ", function, $" to: {this}");

            if (function != null && !Functions.Contains(function))
            {
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
        public virtual bool RemoveFunction(IFunction function)
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
        public virtual bool CanBeTriggered(IEntity entity) => false;

        /// <summary>
        /// Trigger this program. This will start all available <see cref="StartFunctions"/>.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IEntity"/> that triggered this.</param>
        public virtual void Trigger(IEntity triggeredBy)
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

                if (startFunction != null)
                {
                    startFunction.Trigger(null);
                }
                else
                {
                    Log.Instance.Warning("Unable to trigger: ", startFunction, " from ", this, " as it's not in a state to accept triggers.");
                }
            }
        }

        /// <summary>
        /// Attempts to stop program execution by sending the <see cref="IFunction.PolitelyStop"/> call
        /// to every function in the program. It's up to the function to stop cleanly, so it might take some
        /// time to fully stop. See <see cref="Stopping"/> to see the state of this procedure.
        /// </summary>
        public virtual void Stop()
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
        public void SetName(string name) => Name = name;

        /// <summary>
        /// Checks whether the program has been triggered since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the program has been triggered since the given time.</returns>
        public bool HasBeenTriggeredSince(DateTime time) => LastTriggerTime >= time;

        /// <summary>
        /// Checks whether the program has been triggered within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the program has been triggered within the last 5 seconds.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the program has been triggered within the given time.</returns>
        public bool HasBeenTriggeredWithin(TimeSpan time) => LastTriggerTime >= DateTime.Now - time;

        /// <summary>
        /// Checks whether the program has completed a run (successfully or unsuccessfully) since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the program has completed since the given time.</returns>
        public bool HasCompletedSince(DateTime time) => LastCompletionTime >= time;

        /// <summary>
        /// Checks whether the program has completed a run (successfully or unsuccessfully) within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the program has completed within the last 5 seconds.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the program has completed within the given time.</returns>
        public bool HasCompletedWithin(TimeSpan time) => LastCompletionTime >= DateTime.Now - time;

        /// <summary>
        /// Returns a string representation of a program.
        /// </summary>
        /// <returns>A string representation of the program.</returns>
        public override string ToString() => $"[PROGRAM {base.ToString()} [Name: {Name}] [Running: {Running}]]";
    }
}
