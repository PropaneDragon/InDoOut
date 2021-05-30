using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Networking;
using InDoOut_Networking.Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Server
{
    public class Server : AbstractInteractiveNetworkEntity, IServer
    {
        private static readonly SemaphoreSlim _writingSemaphore = new SemaphoreSlim(1, 1);

        private readonly object _clientsLock = new object();
        private readonly object _streamHandlersLock = new object();

        private readonly TcpListener _listener = null;
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private readonly Dictionary<TcpClient, NetworkStreamHandler> _streamHandlers = new Dictionary<TcpClient, NetworkStreamHandler>();

        public bool Started => _listener?.Server?.IsBound ?? false;
        public int Port => (_listener?.LocalEndpoint as IPEndPoint)?.Port ?? 0;
        public IReadOnlyCollection<TcpClient> Clients { get { lock (_clientsLock) { return _clients.ToList().AsReadOnly(); } } }
        public TimeSpan ClientPollInterval { get; set; } = TimeSpan.FromSeconds(5);
        public IPAddress IPAddress => (_listener?.LocalEndpoint as IPEndPoint)?.Address;

        public event EventHandler<ServerConnectionEventArgs> OnServerStarted;
        public event EventHandler<ServerConnectionEventArgs> OnServerStopped;
        public event EventHandler<ClientConnectionEventArgs> OnClientConnected;
        public event EventHandler<ClientConnectionEventArgs> OnClientDisconnected;
        public event EventHandler<ClientMessageEventArgs> OnClientMessageReceived;
        public event EventHandler<ClientMessageEventArgs> OnClientMessageSent;

        private Server()
        {

        }

        public Server(int port = 0) : this()
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Server.NoDelay = true;

            Log.Instance.Header("Created server. IP: ", IPAddress, ", port: ", Port);
        }

        public Server(ILog log, int port = 0) : this(port)
        {
            EntityLog = log;
        }

        public async Task<bool> Start()
        {
            _ = await Stop();

            var started = await Task.Run(() =>
            {
                try
                {
                    _listener.Start();

                    OnServerStarted?.Invoke(this, new ServerConnectionEventArgs());

                    Log.Instance.Header("Started server. IP: ", IPAddress, ", port: ", Port);

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Couldn't start server due to an error: ", ex?.Message);
                }

                return false;
            });

            if (started)
            {
                StartListening();
                StartPolling();

                return true;
            }

            return false;
        }

        public async Task<bool> Stop()
        {
            return await Task.Run(() =>
            {
                var clients = Clients.ToList();

                foreach (var client in clients)
                {
                    _ = DetachClient(client);
                }

                lock (_clientsLock)
                {
                    _clients.Clear();
                }

                try
                {
                    _listener?.Stop();

                    OnServerStopped?.Invoke(this, new ServerConnectionEventArgs());

                    Log.Instance.Header("Stopped server. IP: ", IPAddress, ", port: ", Port);

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Couldn't stop server due to an error: ", ex?.Message);
                }

                return false;
            });
        }

        public async Task<bool> SendMessageAll(string message)
        {
            var clients = Clients;
            var success = true;

            foreach (var client in clients)
            {
                success = await SendMessage(client, message) && success;
            }

            return success;
        }

        public async Task<bool> SendMessage(TcpClient client, string message) => await SendSafe(() => SendUnsafe(client, message));
        public async Task<bool> Ping(TcpClient client) => await SendSafe(() => PingUnsafe(client));

        public override async Task<bool> SendMessage(INetworkMessage command, CancellationToken cancellationToken) => command?.Context is TcpClient client && await SendMessage(client, command.ToString());

        protected async Task ClientMessageReceived(TcpClient client, string message)
        {
            var command = NetworkMessage.FromString(message);
            if (command != null && command.Valid)
            {
                command.Context = client;

                var address = client?.Client?.RemoteEndPoint as IPEndPoint;

                EntityLog.Info(address?.Address, ": Received command - ", command?.Name);

                var response = await ProcessMessage(command, CancellationToken.None);
                if (response?.Valid ?? false)
                {
                    EntityLog.Info(address?.Address, ": Sending response - ", response.Name);
                    
                    _ = await SendMessage(response, CancellationToken.None);
                }
            }
        }

        protected bool CanAcceptClient(TcpClient _) => true;
        protected void ClientConnected(TcpClient _) { }
        protected void ClientDisconnected(TcpClient _) { }

        private void StartListening()
        {
            _ = Task.Run(async () =>
            {
                while (Started)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        if (client != null)
                        {
                            if (CanAcceptClient(client) && AttachClient(client))
                            {
                                ClientConnected(client);

                                OnClientConnected?.Invoke(this, new ClientConnectionEventArgs(client));
                            }
                            else
                            {
                                _ = DetachClient(client);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error("Failed to connect client on server! ", ex?.Message);
                    }
                }
            });
        }

        private void StartPolling()
        {
            _ = Task.Run(async () =>
            {
                while (Started)
                {
                    PollClients();

                    await Task.Delay(ClientPollInterval);
                }
            });
        }

        private bool AttachClient(TcpClient client)
        {
            if (ClientIsValid(client))
            {
                lock (_clientsLock)
                {
                    if (!_clients.Contains(client))
                    {
                        _clients.Add(client);

                        AwaitMessages(client);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool DetachClient(TcpClient client)
        {
            ClientDisconnected(client);

            OnClientDisconnected?.Invoke(this, new ClientConnectionEventArgs(client));

            try
            {
                client?.Close();
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Couldn't stop client due to an error: ", ex?.Message);
            }

            lock (_clientsLock)
            {
                if (_clients.Contains(client))
                {
                    _ = _clients.Remove(client);
                }
            }

            lock (_streamHandlersLock)
            {
                if (_streamHandlers.ContainsKey(client))
                {
                    _ = _streamHandlers.Remove(client);
                }
            }

            return true;
        }

        private void AwaitMessages(TcpClient client)
        {
            _ = Task.Run(async () =>
            {
                while (ClientIsValid(client))
                {
                    try
                    {
                        var stream = client.GetStream();
                        var streamHandler = GetStreamHandlerForClient(client);

                        if (streamHandler != null)
                        {
                            var message = await streamHandler.ReadMessage(stream);

                            if (!string.IsNullOrEmpty(message))
                            {
                                await ClientMessageReceived(client, message);

                                OnClientMessageReceived?.Invoke(this, new ClientMessageEventArgs(client, message));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error("Couldn't receive a message from a client to the server due to an error: ", ex.Message);
                    }
                }
            });
        }

        private void PollClients()
        {
            List<TcpClient> clients = null;

            lock (_clientsLock)
            {
                clients = _clients.ToList();
            }

            foreach (var client in clients)
            {
                var sendTask = Task.Run(async () => await Ping(client));
                _ = sendTask.Result;

                if (!ClientIsValid(client))
                {
                    _ = DetachClient(client);
                }
            }
        }

        private NetworkStreamHandler GetStreamHandlerForClient(TcpClient client)
        {
            if (client != null)
            {
                lock (_streamHandlersLock)
                {
                    if (!_streamHandlers.ContainsKey(client))
                    {
                        _streamHandlers[client] = new NetworkStreamHandler();
                    }

                    return _streamHandlers[client];
                }
            }

            return null;
        }

        private async Task<bool> SendSafe(Func<Task<bool>> safeFunction)
        {
            var taskReturn = false;

            await _writingSemaphore.WaitAsync();

            try
            {
                taskReturn = await safeFunction?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Couldn't send a message to a client from a server due to an error: ", ex.Message);
            }

            _ = _writingSemaphore.Release();

            return taskReturn;
        }

        private async Task<bool> SendUnsafe(TcpClient client, string message)
        {
            if (CanSendMessage(client, message) && await (GetStreamHandlerForClient(client)?.SendMessage(client.GetStream(), message) ?? Task.FromResult(false)))
            {
                OnClientMessageSent?.Invoke(this, new ClientMessageEventArgs(client, message));

                return true;
            }

            return false;
        }

        private async Task<bool> PingUnsafe(TcpClient client) => await (GetStreamHandlerForClient(client)?.SendPing(client.GetStream()) ?? Task.FromResult(false));
        private bool CanSendMessage(TcpClient client, string message) => ClientIsValid(client) && !string.IsNullOrEmpty(message);
        private bool ClientIsValid(TcpClient client) => client?.Connected ?? false;
    }
}
