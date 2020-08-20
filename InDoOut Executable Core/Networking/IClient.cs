using System.Net;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public interface IClient
    {
        bool Connected { get; }

        Task<bool> Connect(IPAddress address, int port);
        Task<bool> Disconnect();
        Task<bool> Send(string message);
    }
}