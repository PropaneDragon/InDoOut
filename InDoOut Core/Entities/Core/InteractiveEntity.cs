using InDoOut_Core.Logging;
using System;
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
    public abstract class InteractiveEntity<ConnectsToType, ConnectsFromType> : Entity, IConnectable<ConnectsToType>, ITriggerable<ConnectsFromType> where ConnectsToType : class, ITriggerable where ConnectsFromType : class, IEntity
    {
        private readonly object _connectionsLock = new object();
        private readonly object _lastTriggerTimeLock = new object();
        private readonly object _lastCompletionTimeLock = new object();

        private Task _runner = null;
        private DateTime _lastTriggerTime = DateTime.MinValue;
        private DateTime _lastCompletionTime = DateTime.MinValue;
        private readonly List<ConnectsToType> _connections = new List<ConnectsToType>();

        /// <summary>
        /// The current running state of this entity.
        /// </summary>
        public bool Running => CheckRunningState();

        /// <summary>
        /// The last time this entity was triggered.
        /// </summary>
        public DateTime LastTriggerTime { get { lock (_lastTriggerTimeLock) return _lastTriggerTime; } }

        /// <summary>
        /// The time this entity last completed a run (successfully or unsuccessfully).
        /// </summary>
        public DateTime LastCompletionTime { get { lock (_lastCompletionTimeLock) return _lastCompletionTime; } }

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
        /// Represents a state where the entity is technically still running, but is in a state
        /// where it is tidying up from the previous trigger, and can accept new triggers.
        /// </summary>
        public virtual bool Finishing => false;

        /// <summary>
        /// Triggers this entity from another entity.
        /// </summary>
        /// <param name="triggeredBy">The entity that triggered this one.</param>
        public void Trigger(ConnectsFromType triggeredBy)
        {
            Log.Instance.Info($"Triggered {this}");

            lock (_lastTriggerTimeLock)
            {
                _lastTriggerTime = DateTime.Now;
            }

            _runner = Task.Run(() =>
            {
                try { Process(triggeredBy); }
                catch (Exception ex) 
                {
                    Log.Instance.Error($"Caught an exception ({ex.Message}) during trigger for {this}");
                }

                lock (_lastCompletionTimeLock)
                {
                    _lastCompletionTime = DateTime.Now;
                }

                Log.Instance.Info($"Completed {this}");
            });
        }

        /// <summary>
        /// Checks whether a given <see cref="IEntity"/> can trigger this.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether the given <see cref="IEntity"/> can trigger this.</returns>
        public bool CanBeTriggered(IEntity entity)
        {
            return CanAcceptConnection(entity) && (!Running || Finishing);
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
        /// Checks whether the entity has been triggered since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the entity has been triggered since the given time.</returns>
        public bool HasBeenTriggeredSince(DateTime time)
        {
            return LastTriggerTime >= time;
        }

        /// <summary>
        /// Checks whether the entity has been triggered within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the entity has been triggered within the last 5 seconds.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the entity has been triggered within the given time.</returns>
        public bool HasBeenTriggeredWithin(TimeSpan time)
        {
            return LastTriggerTime >= DateTime.Now - time;
        }

        /// <summary>
        /// Checks whether the entity has completed a run (successfully or unsuccessfully) since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the entity has completed since the given time.</returns>
        public bool HasCompletedSince(DateTime time)
        {
            return LastTriggerTime >= time;
        }

        /// <summary>
        /// Checks whether the entity has completed a run (successfully or unsuccessfully) within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the entity has completed within the last 5 seconds.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the entity has completed within the given time.</returns>
        public bool HasCompletedWithin(TimeSpan time)
        {
            return LastTriggerTime >= DateTime.Now - time;
        }

        /// <summary>
        /// A string representation of this entity.
        /// </summary>
        /// <returns>A string representation of this entity.</returns>
        public override string ToString()
        {
            return $"{base.ToString()} [Running: {Running}] [Connections: {Connections.Count}]";
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

                    Log.Instance.Info($"Connection added: {this} ++++++++++ ", connection);

                    return true;
                }

                Log.Instance.Error($"Connection failed: {this} ++++++++++ ", connection);

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
                Log.Instance.Info($"Connection removed: {this} ---------- ", connection);

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

        private bool CheckRunningState()
        {
            return _runner?.Status switch
            {
                TaskStatus.Created => true,
                TaskStatus.Running => true,
                TaskStatus.WaitingForActivation => true,
                TaskStatus.WaitingForChildrenToComplete => true,
                TaskStatus.WaitingToRun => true,
                _ => false
            };
        }
    }
}
