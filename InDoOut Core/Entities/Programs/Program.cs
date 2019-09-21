using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
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
    public class Program : IProgram
    {
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
        public bool Running => Functions.Any(function => function.Running);

        /// <summary>
        /// The name of this program.
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// The unique ID for this program.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The current variable store for all program variables.
        /// </summary>
        /// <seealso cref="VariableStore"/>
        public IVariableStore VariableStore { get; protected set; } = new VariableStore();

        /// <summary>
        /// Metadata associated with the program.
        /// </summary>
        public Dictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a program with optional passthrough values.
        /// </summary>
        /// <param name="passthroughValues">Values to pass into the <see cref="IStartFunction"/>s when triggered.</param>
        /// <seealso cref="Trigger(IEntity)"/>
        public Program(params string[] passthroughValues)
        {
            PassthroughValues = passthroughValues.ToList();
        }

        /// <summary>
        /// Add a function to the program.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <returns>Whether the function was added.</returns>
        public bool AddFunction(IFunction function)
        {
            if (function != null && !Functions.Contains(function))
            {
                function.VariableStore = VariableStore;

                Functions.Add(function);

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
        /// Sets the program name to the given value.
        /// </summary>
        /// <param name="name">The program name to set.</param>
        public void SetName(string name)
        {
            Name = name;
        }
    }
}
