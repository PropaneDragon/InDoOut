using System.Collections.Generic;

namespace InDoOut_UI_Common.Storage.Options
{
    internal struct ConnectionItem
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        public bool Valid => Address != null;
    }

    internal class ConnectionOptionStorage
    {
        public List<ConnectionItem> Connections { get; set; } = new List<ConnectionItem>();
    }
}
