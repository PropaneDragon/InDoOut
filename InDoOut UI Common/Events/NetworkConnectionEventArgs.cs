using System;

namespace InDoOut_UI_Common.Events
{
    public class NetworkConnectionEventArgs : EventArgs
    {
        public string Address { get; set; } = null;
        public int Port { get; set; } = 0;

        public NetworkConnectionEventArgs()
        {
        }

        public NetworkConnectionEventArgs(string address, int port)
        {
            Address = address;
            Port = port;
        }
    }
}
