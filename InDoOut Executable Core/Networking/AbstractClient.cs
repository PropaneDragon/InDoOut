using InDoOut_Core.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public abstract class AbstractClient : IClient
    {
        private static readonly SemaphoreSlim _writingSemaphore = new SemaphoreSlim(1, 1);

        private readonly NetworkStreamHandler _streamHandler = new NetworkStreamHandler();

        private TcpClient _client = null;

        public bool Connected => _client?.Connected ?? false;

        public AbstractClient()
        {
        }

        public async Task<bool> Connect(IPAddress address, int port)
        {
            _ = await Disconnect();

            try
            {
                _client = new TcpClient() { NoDelay = true };

                await _client.ConnectAsync(address, port);

                if (Connected)
                {
                    AwaitMessages();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Client couldn't connect to server at ", address?.ToString(), ":", port, " due to error: ", ex?.Message);
            }

            return false;
        }

        public async Task<bool> Disconnect()
        {
            return Connected && await Task.Run(() =>
            {
                try
                {
                    _client.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Client couldn't disconnect from server due to an error: ", ex?.Message);
                }

                return false;
            });
        }

        public async Task<bool> Send(string message)
        {
            var wrote = false;

            if (Connected && !string.IsNullOrEmpty(message))
            {
                await _writingSemaphore.WaitAsync();

                try
                {
                    var stream = _client.GetStream();

                    wrote = await _streamHandler.SendMessage(stream, message);
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Couldn't send a message to the server due to an error: ", ex.Message);
                }

                _ = _writingSemaphore.Release();
            }

            return wrote;
        }

        protected abstract void MessageReceived(string message);

        private void AwaitMessages()
        {
            _ = Task.Run(async () =>
            {
                while (Connected)
                {
                    try
                    {
                        var stream = _client.GetStream();
                        var message = await _streamHandler.ReadMessage(stream);

                        if (MessageIsValid(message))
                        {
                            MessageReceived(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error("Couldn't receive a message from the server due to an error: ", ex.Message);
                    }
                }
            });
        }

        private bool MessageIsValid(string message) => !string.IsNullOrEmpty(message);
    }
}
