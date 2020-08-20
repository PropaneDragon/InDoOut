using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public class NetworkStreamHandler
    {
        public static readonly string MESSAGE_ALIVE_CHECK = "\u0001\u0001\u0003";

        public string MessageBeginIdentifier { get; set; } = "\u0001\u0002\u0003";
        public string MessageEndIdentifier { get; set; } = "\u0003\u0002\u0001";

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
                var fullString = $"{MessageBeginIdentifier}{message}{MessageEndIdentifier}";

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
                    
                    if (string.IsNullOrEmpty(message) || message.Contains(MessageEndIdentifier))
                    {
                        end = true;
                    }

                    fullMessage += message ?? "";
                }

                var beginningLocation = fullMessage.IndexOf(MessageBeginIdentifier);
                var endingLocation = fullMessage.IndexOf(MessageEndIdentifier);

                beginningLocation = beginningLocation >= 0 ? beginningLocation + MessageBeginIdentifier.Length : 0;
                endingLocation = endingLocation >= 0 ? endingLocation : fullMessage.Length;

                return SanitiseMessage(fullMessage[beginningLocation..endingLocation]);
            }

            return null;
        }

        public async Task<bool> SendPing(NetworkStream stream) => await SendMessage(stream, MESSAGE_ALIVE_CHECK);

        protected virtual string SanitiseMessage(string message) => !string.IsNullOrEmpty(message) && message != MESSAGE_ALIVE_CHECK ? message : null;
    }
}
