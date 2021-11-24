using InDoOut_Core.Logging;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public class NetworkStreamHandler
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public NetworkStreamHandler()
        {
        }

        public async Task<bool> SendMessage(NetworkStream stream, string message)
        {
            if (stream?.CanWrite ?? false)
            {
                stream.WriteTimeout = (int)Math.Round(TimeSpan.FromSeconds(1).TotalMilliseconds);

                using var writer = new StreamWriter(stream, Encoding, leaveOpen: true);
                var fullString = $"{NetworkCodes.MESSAGE_BEGIN_IDENTIFIER}{message}{NetworkCodes.MESSAGE_END_IDENTIFIER}";

                await writer.WriteLineAsync(fullString);

                return true;
            }

            return false;
        }

        public async Task<string> ReadMessage(NetworkStream stream)
        {
            if (stream?.CanRead ?? false)
            {
                stream.ReadTimeout = (int)Math.Round(TimeSpan.FromSeconds(5).TotalMilliseconds);

                using var reader = new StreamReader(stream, Encoding, leaveOpen: true);

                var end = false;
                var fullMessage = "";

                while (!end)
                {
                    var message = (await reader.ReadLineAsync()) + "\n";

                    if (string.IsNullOrEmpty(message) || message.Contains(NetworkCodes.MESSAGE_END_IDENTIFIER))
                    {
                        end = true;
                    }

                    fullMessage += message ?? "";
                }

                var beginningLocation = fullMessage.IndexOf(NetworkCodes.MESSAGE_BEGIN_IDENTIFIER);
                var endingLocation = fullMessage.IndexOf(NetworkCodes.MESSAGE_END_IDENTIFIER);

                beginningLocation = beginningLocation >= 0 ? beginningLocation + NetworkCodes.MESSAGE_END_IDENTIFIER.Length : 0;
                endingLocation = endingLocation >= 0 ? endingLocation : fullMessage.Length;

                if (beginningLocation <= endingLocation)
                {
                    return SanitiseMessage(fullMessage[beginningLocation..endingLocation]);
                }
                else
                {
                    Log.Instance.Info("End of message was before start! M: ", fullMessage, ", B: ", beginningLocation, ", E: ", endingLocation);
                }
            }

            return null;
        }

        public async Task<bool> SendPing(NetworkStream stream) => await SendMessage(stream, NetworkCodes.MESSAGE_PING_IDENTIFIER);

        protected virtual string SanitiseMessage(string message) => !string.IsNullOrEmpty(message) && message != NetworkCodes.MESSAGE_PING_IDENTIFIER ? message : null;
    }
}
