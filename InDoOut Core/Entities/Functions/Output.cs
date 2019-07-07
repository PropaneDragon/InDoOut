using InDoOut_Core.Entities.Core;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Outputs are triggered by <see cref="IFunction"/>s. These outputs are simple connections for
    /// <see cref="IInput"/>s which then trigger their connected entity.
    /// </summary>
    public class Output : InteractiveEntity<IInput, IFunction>, IOutput
    {
        /// <summary>
        /// The name of this output.
        /// </summary>
        public string Name { get; protected set; } = "Output";
        
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
        public bool Connect(IInput input)
        {
            return AddConnection(input);
        }

        /// <summary>
        /// Disconnects this output from a given <see cref="IInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="IInput"/> to disconnect from.</param>
        /// <returns>Whether the connection was disconnected.</returns>
        public bool Disconnect(IInput input)
        {
            return RemoveConnection(input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Output output && Name == output.Name;
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
        /// Begins processing this output. This calls all the <see cref="IInput"/>s it is connected to.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IFunction"/> that triggered this output.</param>
        protected override void Process(IFunction triggeredBy)
        {
            foreach (var connection in Connections)
            {
                if (connection != null && connection.CanBeTriggered(this))
                {
                    connection.Trigger(this);
                }
            }
        }
    }
}
