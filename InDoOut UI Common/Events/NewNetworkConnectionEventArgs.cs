using System;

namespace InDoOut_UI_Common.Events
{
    public class NewNetworkConnectionEventArgs : EventArgs
    {
        public int Port { get; private set; } = 0;
        public string Address { get; private set; } = null;

        public NewNetworkConnectionEventArgs(string address, int port)
        {
            Address = address;
            Port = port;
        }
    }
}
