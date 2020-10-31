using InDoOut_Core.Entities.Core;
using InDoOut_Core.Logging;
using System.Linq;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Outputs are triggered by <see cref="IFunction"/>s. These outputs are simple connections for
    /// <see cref="IInput"/>s which then trigger their connected entity.
    /// </summary>
    public abstract class Output : InteractiveEntity<IInput, IFunction>, IOutput
    {
        /// <summary>
        /// The name of this output.
        /// </summary>
        public string Name { get; protected set; } = "Output";

        /// <summary>
        /// Whether this output is currently tidying up and ready to accept
        /// a new trigger, even if it's technically still running.
        /// </summary>
        public override bool Finishing => true;

        /// <summary>
        /// Creates a basic output with a name.
        /// </summary>
        /// <param name="name">The name to give the output.</param>
        public Output(string name = "Output")
        {
            Name = name;
        }

        /// <summary>
        /// Connects this output to a given <see cref="IInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="IInput"/> to connect to.</param>
        /// <returns>Whether the connection was made.</returns>
        public bool Connect(IInput input) => AddConnection(input);

        /// <summary>
        /// Disconnects this output from a given <see cref="IInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="IInput"/> to disconnect from.</param>
        /// <returns>Whether the connection was disconnected.</returns>
        public bool Disconnect(IInput input) => RemoveConnection(input);

        /// <summary>
        /// A string representation of this entity.
        /// </summary>
        /// <returns>A string representation of this entity.</returns>
        public override string ToString() => $"[OUTPUT {base.ToString()} [Name: {Name ?? "null"}]]";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is Output output && Name == output.Name && Connections.Count == output.Connections.Count && Connections.All(connection => output.Connections.Contains(connection));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Begins processing this output. This calls all the <see cref="IInput"/>s it is connected to.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IFunction"/> that triggered this output.</param>
        protected override void Process(IFunction triggeredBy)
        {
            Log.Instance.Info($"Processing {this}");

            foreach (var connection in Connections)
            {
                if (connection != null && connection.CanBeTriggered(this))
                {
                    Log.Instance.Info($"Triggering: {this} >>>>>>>>>> ", connection);

                    connection.Trigger(this);
                }
                else
                {
                    Log.Instance.Warning("Unable to trigger: ", connection, " from ", this, " as it's not in a state to accept triggers.");
                }
            }

            Log.Instance.Info($"Processed {this}");
        }
    }
}
