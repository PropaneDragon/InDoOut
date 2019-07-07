using InDoOut_Core.Entities.Core;

namespace InDoOut_Core_Tests
{
    internal class TestInteractiveEntity : InteractiveEntity<ITriggerable, IEntity>
    {
        public bool Processed { get; set; } = false;
        public IEntity LastTriggeredBy { get; protected set; } = null;

        public bool AddConnectionPublic(ITriggerable connection) => AddConnection(connection);
        public bool AddConnectionsPublic(params ITriggerable[] connections) => AddConnections(connections);
        public bool RemoveConnectionPublic(ITriggerable connection) => RemoveConnection(connection);
        public bool RemoveConnectionsPublic(params ITriggerable[] connections) => RemoveConnections(connections);
        public bool RemoveAllConnectionsPublic() => RemoveAllConnections();
        public bool SetConnectionPublic(params ITriggerable[] connections) => SetConnection(connections);
        public void ProcessPublic(IEntity triggeredBy) => Process(triggeredBy);

        protected override void Process(IEntity triggeredBy)
        {
            Processed = true;
            LastTriggeredBy = triggeredBy;
        }
    }
}
