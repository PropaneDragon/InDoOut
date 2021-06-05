using InDoOut_Core.Logging;
using System.Net;
using System.Net.Sockets;

namespace InDoOut_Networking.Shared
{
    public static class PortFinder
    {
        public static int Find()
        {
            var foundPort = -1;
            var listener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                Log.Instance.Info("Finding available port");

                listener.Start();

                foundPort = (listener.LocalEndpoint as IPEndPoint).Port;
            }
            catch
            {
                Log.Instance.Warning("Couldn't find any open ports.");
            }
            finally
            {
                listener.Stop();
            }

            return foundPort;
        }

        public static int Find(int start, int end)
        {
            var foundPort = false;

            Log.Instance.Info("Scanning for open ports between ", start, " and ", end);

            for (var currentPort = start; currentPort < end; ++currentPort)
            {
                var listener = new TcpListener(IPAddress.Loopback, currentPort);

                try
                {
                    Log.Instance.Info("Checking port ", currentPort);

                    listener.Start();

                    foundPort = true;
                }
                catch
                {
                    Log.Instance.Warning("Attempted to start on port ", currentPort, " but failed. Trying next.");
                }
                finally
                {
                    listener.Stop();
                }

                if (foundPort)
                {
                    return currentPort;
                }
            }

            return -1;
        }
    }
}
