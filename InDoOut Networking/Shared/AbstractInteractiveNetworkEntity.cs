using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Networking.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public abstract class AbstractInteractiveNetworkEntity : IInteractiveNetworkEntity
    {
        private readonly object _responseQueueLock = new object();
        private readonly object _commandListenersLock = new object();
        private readonly List<ICommandListener> _commandListeners = new List<ICommandListener>();
        private readonly Dictionary<string, INetworkMessage> _responseQueue = new Dictionary<string, INetworkMessage>();

        public ILog EntityLog { get; protected set; } = new NullLog();

        public abstract Task<bool> SendMessage(INetworkMessage command, CancellationToken cancellationToken);

        public async Task<INetworkMessage> SendMessageAwaitResponse(INetworkMessage message, CancellationToken cancellationToken)
        {
            if (message != null && message.Valid && !cancellationToken.IsCancellationRequested)
            {
                _ = AddToResponseQueue(message);

                if (await SendMessage(message, cancellationToken))
                {
                    return await AwaitMessageResponse(message, cancellationToken);
                }
                else
                {
                    _ = RemoveFromResponseQueue(message);
                }
            }

            return null;
        }

        public bool AddCommandListener(ICommandListener listener)
        {
            lock (_commandListenersLock)
            {
                if (!string.IsNullOrWhiteSpace(listener?.CommandName) && !_commandListeners.Contains(listener) && _commandListeners.FirstOrDefault(existingListener => existingListener.CommandName == listener.CommandName) == null)
                {
                    _commandListeners.Add(listener);

                    return true;
                }
            }

            return false;
        }

        protected virtual async Task<INetworkMessage> ProcessMessage(INetworkMessage message, CancellationToken cancellationToken)
        {
            if (message != null && !cancellationToken.IsCancellationRequested)
            {
                if (MessageAwaitingResponse(message))
                {
                    _ = RespondToQueuedMessage(message);
                }
                else
                {
                    return await NewMessageReceived(message, cancellationToken);
                }
            }

            return null;
        }

        private async Task<INetworkMessage> NewMessageReceived(INetworkMessage message, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(message?.Name) && !cancellationToken.IsCancellationRequested)
            {
                ICommandListener command = null;

                lock (_commandListenersLock)
                {
                    command = _commandListeners.FirstOrDefault(commandListener => commandListener.CommandName == message.Name);
                }

                if (command != null)
                {
                    return await command.CommandReceived(message, cancellationToken);
                }
            }

            return null;
        }

        private async Task<INetworkMessage> AwaitMessageResponse(INetworkMessage message, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                INetworkMessage response = null;

                while (!cancellationToken.IsCancellationRequested)
                {
                    response = GetResponseForMessage(message);

                    if (response == null)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(50));
                    }
                    else
                    {
                        break;
                    }
                }

                _ = RemoveFromResponseQueue(message);

                return response;
            });
        }

        private bool MessageAwaitingResponse(INetworkMessage message)
        {
            lock (_responseQueueLock)
            {
                return _responseQueue.ContainsKey(message.Id);
            }
        }

        private INetworkMessage GetResponseForMessage(INetworkMessage message)
        {
            if (message?.Id != null)
            {
                lock (_responseQueueLock)
                {
                    if (_responseQueue.ContainsKey(message.Id))
                    {
                        return _responseQueue[message.Id];
                    }
                }
            }

            return null;
        }

        private bool AddToResponseQueue(INetworkMessage message)
        {
            if (message?.Id != null)
            {
                lock (_responseQueueLock)
                {
                    if (!_responseQueue.ContainsKey(message.Id))
                    {
                        _responseQueue[message.Id] = null;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool RespondToQueuedMessage(INetworkMessage message)
        {
            if (message?.Id != null)
            {
                lock (_responseQueueLock)
                {
                    if (_responseQueue.ContainsKey(message.Id))
                    {
                        _responseQueue[message.Id] = message;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool RemoveFromResponseQueue(INetworkMessage message)
        {
            if (message?.Id != null)
            {
                lock (_responseQueueLock)
                {
                    if (message.Id != null && _responseQueue.ContainsKey(message.Id))
                    {
                        _ = _responseQueue.Remove(message.Id);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
