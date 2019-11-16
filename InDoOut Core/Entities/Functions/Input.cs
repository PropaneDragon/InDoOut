using InDoOut_Core.Entities.Core;
using InDoOut_Core.Logging;
using System.Linq;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Inputs are triggered by <see cref="IOutput"/>s. These intputs are simple connections for
    /// <see cref="IFunction"/>s which can trigger the code within them. The <see cref="IFunction"/> may
    /// perform different actions based on the type of input that was triggered.
    /// </summary>
    public class Input : InteractiveEntity<IFunction, IOutput>, IInput
    {
        /// <summary>
        /// The name of this input.
        /// </summary>
        public string Name { get; protected set; } = "Input";

        /// <summary>
        /// The parent this input belongs to.
        /// </summary>
        public IFunction Parent => Connections.FirstOrDefault();

        /// <summary>
        /// Whether this input is currently tidying up and ready to accept
        /// a new trigger, even if it's technically still running.
        /// </summary>
        public override bool Finishing => true;

        private Input() { }

        /// <summary>
        /// Creates a generic input with a parent.
        /// </summary>
        /// <param name="parent">The <see cref="IFunction"/> this input belongs to.</param>
        /// <param name="name">The name of this input.</param>
        public Input(IFunction parent, string name = "Input")
        {
            _ = SetConnection(parent);
            Name = name;
        }

        /// <summary>
        /// A string representation of this entity.
        /// </summary>
        /// <returns>A string representation of this entity.</returns>
        public override string ToString()
        {
            return $"[INPUT {base.ToString()} [Name: {Name ?? "null"}]]";
        }

        /// <summary>
        /// Checks whether an input is equal to another input.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>Whether a given object is equal to this input.</returns>
        public override bool Equals(object obj)
        {
            return obj is Input input && Name == input.Name && Connections.Count == input.Connections.Count && Connections.All(connection => input.Connections.Contains(connection));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Begins processing this input. This calls the <see cref="Parent"/> it is connected to.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IOutput"/> that triggered this input.</param>
        protected override void Process(IOutput triggeredBy)
        {
            Log.Instance.Info($"Processing {this}");

            if (Parent != null && Parent.CanBeTriggered(this))
            {
                Log.Instance.Info($"Triggering: {this} >>>>>>>>>> ", Parent);

                Parent.Trigger(this);
            }
            else
            {
                Log.Instance.Warning("Unable to trigger: ", Parent, " from ", this, " as it's not in a state to accept triggers.");
            }

            Log.Instance.Info($"Processed {this}");
        }
    }
}
