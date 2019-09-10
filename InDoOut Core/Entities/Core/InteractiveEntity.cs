using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// An entity that can be connected to and triggered by another <see cref="IEntity"/>.
    /// </summary>
    /// <typeparam name="ConnectsToType">The <see cref="ITriggerable"/> that this entity can connect to.</typeparam>
    /// <typeparam name="ConnectsFromType">The <see cref="IEntity"/> that this entity can accept connections from.</typeparam>
    public abstract class InteractiveEntity<ConnectsToType, ConnectsFromType> : NamedEntity, IConnectable<ConnectsToType>, ITriggerable<ConnectsFromType> where ConnectsToType : class, ITriggerable where ConnectsFromType : class, IEntity
    {
        private readonly object _connectionsLock = new object();

        private Task _runner = null;
        private readonly List<ConnectsToType> _connections = new List<ConnectsToType>();

        /// <summary>
        /// The current running state of this entity.
        /// </summary>
        public bool Running => _runner != null && (_runner.Status == TaskStatus.Running || _runner.Status == TaskStatus.WaitingToRun || _runner.Status == TaskStatus.WaitingForChildrenToComplete || _runner.Status == TaskStatus.WaitingForActivation);

        /// <summary>
        /// The connections that this entity has.
        /// </summary>
        public List<ITriggerable> RawConnections => Connections.Cast<ITriggerable>().ToList();

        /// <summary>
        /// The connections that this entity has.
        /// </summary>
        public List<ConnectsToType> Connections
        {
            get { lock (_connectionsLock) return _connections; }
        }

        /// <summary>
        /// Triggers this entity from another entity.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this one.</param>
        public void Trigger(ConnectsFromType triggeredBy)
        {
            _runner = Task.Run(() =>
            {
                try { Process(triggeredBy); }
                catch { }
            });
        }

        /// <summary>
        /// Checks whether a given <see cref="IEntity"/> can trigger this.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether the given <see cref="IEntity"/> can trigger this.</returns>
        public bool CanBeTriggered(IEntity entity)
        {
            return CanAcceptConnection(entity) && !Running;
        }

        /// <summary>
        /// Checks whether a given <see cref="IEntity"/> can connect to this.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether the given <see cref="IEntity"/> can trigger this.</returns>
        public bool CanAcceptConnection(IEntity entity)
        {
            return entity != null && entity != this && typeof(ConnectsFromType).IsAssignableFrom(entity.GetType());
        }

        /// <summary>
        /// Adds a connection to the entity.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        /// <returns>Whether the connection was added.</returns>
        protected bool AddConnection(ConnectsToType connection)
        {
            lock (_connectionsLock)
            {
                if (connection != null && connection != this && !_connections.Contains(connection))
                {
                    _connections.Add(connection);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Adds numerous connections to the entity.
        /// </summary>
        /// <param name="connections">The connections to add.</param>
        /// <returns>Whether all connections were added.</returns>
        protected bool AddConnections(params ConnectsToType[] connections)
        {
            var connectedAll = true;

            foreach (var connection in connections)
            {
                connectedAll = AddConnection(connection) && connectedAll;
            }

            return connectedAll;
        }

        /// <summary>
        /// Removes a connection from the entity.
        /// </summary>
        /// <param name="connection">The connection to remove.</param>
        /// <returns>Whether the connection was found and removed.</returns>
        protected bool RemoveConnection(ConnectsToType connection)
        {
            lock (_connectionsLock)
            {
                return connection != null && _connections.Contains(connection) ? _connections.Remove(connection) : false;
            }
        }

        /// <summary>
        /// Removes numerous connections from the entity.
        /// </summary>
        /// <param name="connections">The connectsions to remove.</param>
        /// <returns>Whether all connections were removed.</returns>
        protected bool RemoveConnections(params ConnectsToType[] connections)
        {
            var removedAll = true;

            foreach (var connection in connections)
            {
                removedAll = RemoveConnection(connection) && removedAll;
            }

            return removedAll;
        }

        /// <summary>
        /// Removes all connections from the entity.
        /// </summary>
        /// <returns>Whether all connections were removed.</returns>
        protected bool RemoveAllConnections()
        {
            lock (_connectionsLock)
            {
                _connections.Clear();

                return true;
            }
        }

        /// <summary>
        /// Sets the current connections to the given connections. This removes
        /// all current connections.
        /// </summary>
        /// <param name="connections">The connections to set.</param>
        /// <returns>Whether the connections were set.</returns>
        protected bool SetConnection(params ConnectsToType[] connections)
        {
            return RemoveAllConnections() && AddConnections(connections);
        }

        /// <summary>
        /// Begins processing after being triggered by a connected entity.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this.</param>
        protected abstract void Process(ConnectsFromType triggeredBy);
    }
}
