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
        TimeSpan FastUpdateInterval { get; set; }
        TimeSpan SlowUpdateInterval { get; set; }

        IClient AssociatedClient { get; }

        bool UpdateFromStatus(ProgramStatus status, bool clearAllFirst = false);

        Task<bool> Reload(CancellationToken cancellationToken);
        Task<bool> Synchronise(CancellationToken cancellationToken);
        Task<bool> Disconnect();
    }
}