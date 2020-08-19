using InDoOut_Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    //http://zetcode.com/csharp/tcpclient/

    public abstract class Server
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

        private Server()
        {

        }
        
        public Server(int port = 0) : this()
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Server.NoDelay = true;

            Log.Instance.Header("Started server. IP: ", IPAddress, ", port: ", Port);
        }

        public async Task<bool> Start()
        {
            _ = await Stop();

            var started = await Task.Run(() =>
            {
                try
                {
                    _listener.Start();

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

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Couldn't stop server due to an error: ", ex?.Message);
                }

                return false;
            });
        }

        public async Task<bool> SendAll(string message)
        {
            var clients = Clients;
            var success = true;

            foreach (var client in clients)
            {
                success = await Send(message, client) && success;
            }

            return success;
        }

        public async Task<bool> Send(string message, TcpClient client)
        {
            var wrote = false;

            if (ClientIsValid(client) && !string.IsNullOrEmpty(message))
            {
                await _writingSemaphore.WaitAsync();

                try
                {
                    var stream = client.GetStream();
                    var streamHandler = GetStreamHandlerForClient(client);

                    if (streamHandler != null)
                    {
                        wrote = await streamHandler.SendMessage(stream, message);
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Couldn't send a message to a client from a server due to an error: ", ex.Message);
                }

                _ = _writingSemaphore.Release();
            }

            return wrote;
        }

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
            try
            {
                client?.Close();
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Couldn't stop client due to an error: ", ex?.Message);
            }

            ClientDisconnected(client);

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
                                ClientMessageReceived(client, message);
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
                _ = Task.Run(async () => await Send("", client));

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

        private bool ClientIsValid(TcpClient client) => client?.Connected ?? false;

        protected abstract bool CanAcceptClient(TcpClient client);
        protected abstract void ClientConnected(TcpClient client);
        protected abstract void ClientDisconnected(TcpClient client);
        protected abstract void ClientMessageReceived(TcpClient client, string message);
    }
}
