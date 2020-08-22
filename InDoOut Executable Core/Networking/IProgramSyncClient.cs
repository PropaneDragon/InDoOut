using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public interface IProgramSyncClient : IClient
    {
        Task<List<string>> RequestAvailablePrograms();
        Task<List<string>> RequestAvailablePrograms(CancellationToken cancellationToken);
    }
}