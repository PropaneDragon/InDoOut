using InDoOut_Core.Entities.Programs;
using InDoOut_Networking.Client;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Entities
{
    public interface INetworkedProgram : IProgram
    {
        bool Connected { get; }

        DateTime LastUpdateTime { get; }

        IClient AssociatedClient { get; }

        bool UpdateFromStatus(ProgramStatus status);

        Task<bool> Reload(CancellationToken cancellationToken);
        Task<bool> Synchronise(CancellationToken cancellationToken);
        Task<bool> Disconnect();
    }
}