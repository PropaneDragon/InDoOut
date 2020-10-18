using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;

namespace InDoOut_Executable_Core.Networking.Entities
{
    public class NetworkedProgram : Program, INetworkedProgram
    {
        private readonly IClient _associatedClient = null;

        public override string Name { get => base.Name != null ? $"{base.Name} [{(Connected ? "Connected" : "Disconnected")}]" : null; protected set => base.Name = value; }

        public bool Connected => _associatedClient?.Connected ?? false;

        private NetworkedProgram(params string[] passthroughValues) : base(passthroughValues)
        {
        }

        public NetworkedProgram(IClient client) : this()
        {
            _associatedClient = client;
        }

        public override bool AddFunction(IFunction function) => false;
        public override bool CanBeTriggered(IEntity entity) => false;
        public override bool RemoveFunction(IFunction function) => false;
        public override void Stop() { }
        public override void Trigger(IEntity triggeredBy) { }
    }
}
