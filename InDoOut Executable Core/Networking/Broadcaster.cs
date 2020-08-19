using InDoOut_Core.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    //https://stackoverflow.com/questions/40616911/c-sharp-udp-broadcast-and-receive-example
    //https://github.com/dotnet/runtime/issues/25269

    public class Broadcaster
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private UdpClient _client = null;
        private IPEndPoint _endpoint = null;

        public bool Connected => _client?.Client?.IsBound ?? false;

        public event EventHandler<EventArgs> BroadcasterStarted;
        public event EventHandler<EventArgs> BroadcasterEnded;
        public event EventHandler<EventArgs> BroadcasterDataSent;
        public event EventHandler<EventArgs> BroadcasterDataReceived;
        public event EventHandler<EventArgs> BroadcasterError;

        public Broadcaster()
        {
        }

        public async Task<bool> Begin(IPAddress ipAddress, int port, bool broadcast = false)
        {
            Log.Instance.Header("Starting up broadcaster at address: ", ipAddress, " on port ", port, " with broadcast: ", broadcast);

            if (Connected)
            {
                End();
            }

            _client = new UdpClient() { EnableBroadcast = broadcast };
            _endpoint = new IPEndPoint(ipAddress, port);

            return await Task.Run(() =>
            {
                try
                {
                    Log.Instance.Info("Attempting to bind to port...");

                    _client.Client.Bind(new IPEndPoint(IPAddress.Any, port));

                    Log.Instance.Info("Bound: ", _client?.Client?.IsBound);

                    BroadcasterStarted?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Failed to bind to port ", port, "! ", ex.Message);

                    BroadcasterError?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.

                    End();

                    return false;
                }
            });
        }

        public void End()
        {
            Log.Instance.Info("Shutting down broadcaster at ", _endpoint?.Address, " on port ", _endpoint?.Port, " with broadcast: ", _client?.EnableBroadcast);

            if (Connected)
            {
                _endpoint = null;

                _client?.Close();
            }

            _client?.Dispose();

            BroadcasterEnded?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.

            Log.Instance.Info("Broadcaster stopped");
        }

        public async Task<bool> Send(string message)
        {
            Log.Instance.Info("Attempting to send a message to client(s) at ", _endpoint?.Address, " on port ", _endpoint?.Port, " with broadcast: ", _client?.EnableBroadcast);

            if (Connected && !string.IsNullOrEmpty(message))
            {
                var data = _encoding.GetBytes(message);
                
                var bytesToSend = data.Length;
                try
                {
                    var bytesSent = await _client.SendAsync(data, data.Length, _endpoint);
                    var bytesEqual = bytesToSend == bytesSent;

                    Log.Instance.Info("Message sent. ", bytesToSend, " bytes to send, ", bytesSent, " bytes sent.");

                    if (bytesEqual)
                    {
                        BroadcasterDataSent?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.
                    }
                    else
                    {
                        Log.Instance.Error("Message didn't send the full amount of bytes!");

                        BroadcasterError?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.
                    }

                    return bytesEqual;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Message couldn't be sent! ", ex.Message);

                    BroadcasterError?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.
                }
            }

            return false;
        }

        public async Task<string> Listen()
        {
            Log.Instance.Info("Listening for messages at ", _endpoint?.Address, " on port ", _endpoint?.Port, " with broadcast: ", _client?.EnableBroadcast);

            if (Connected)
            {
                try
                {
                    var result = await _client.ReceiveAsync();
                    var data = result.Buffer;

                    if (data != null)
                    {
                        var @string = _encoding.GetString(data);
                        if (!string.IsNullOrEmpty(@string))
                        {
                            Log.Instance.Info("Message received. ", @string.Length, " characters long");

                            BroadcasterDataReceived?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.

                            return @string;
                        }
                        else
                        {
                            Log.Instance.Error("Message received was empty!");

                            BroadcasterError?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Message couldn't be sent! ", ex.Message);

                    BroadcasterError?.Invoke(this, new EventArgs()); //Todo: Needs EventArgs.
                }
            }

            return null;
        }

        public void StartListening()
        {
            Log.Instance.Info("Starting a continuous listener at ", _endpoint?.Address, " on port ", _endpoint?.Port, " with broadcast: ", _client?.EnableBroadcast);

            _ = Task.Run(async () =>
            {
                while (Connected)
                {
                    _ = await Listen();
                }
            });
        }
    }
}
